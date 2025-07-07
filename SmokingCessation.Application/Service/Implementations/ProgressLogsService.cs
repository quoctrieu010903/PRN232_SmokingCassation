
using System.Globalization;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SmokingCessation.Application.DTOs.Fillter;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.CustomExceptionss;
using SmokingCessation.Core.Response;
using SmokingCessation.Core.Utils;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Enums;
using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Domain.Specifications;

namespace SmokingCessation.Application.Service.Implementations
{
    public class ProgressLogsService : IProgressLogsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAchievementService _userAchivement;

        public ProgressLogsService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUserAchievementService userAchivement)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userAchivement = userAchivement;
        }

        public async Task<BaseResponseModel> Create(ProgressLogsRequest request)
        {   // 1. Lấy userId từ context
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Không xác định được người dùng.");

            var quitPlan = await GetUserActiveOrLatestQuitPlan(Guid.Parse(userId));
           if (quitPlan == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);
            }
            var entity = _mapper.Map<ProgressLog>(request);
            entity.QuitPlanId = quitPlan.Id; 
            entity.LogDate =  DateTime.UtcNow;
            entity.CreatedTime =  DateTime.UtcNow;
            entity.LastUpdatedTime =  DateTime.UtcNow;  

            await _unitOfWork.Repository<ProgressLog, ProgressLog>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            await _userAchivement.AssignAchievementsIfEligibleAsync(Guid.Parse(userId));

            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.CREATE_SUCCESS);
        }

        public async Task<BaseResponseModel> CreateProgressLogFromAdviceAsync()
        {
            // 1. Lấy userId từ context
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Không xác định được người dùng.");

             var quitPlan = await GetUserActiveOrLatestQuitPlan(Guid.Parse(userId));

            if (quitPlan == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);
            }

            // 3. Lấy lời khuyên mới nhất cho kế hoạch này
            var adviceLogs = await _unitOfWork.Repository<CoachAdviceLog, CoachAdviceLog>().GetAllAsync();
            var latestAdvice = adviceLogs
                .Where(a => a.QuitPlanId == quitPlan.Id)
                .OrderByDescending(a => a.AdviceDate)
                .FirstOrDefault();

            // 4. Kiểm tra đã có ProgressLog cho ngày hôm nay chưa
            var today =  DateTime.UtcNow.Date;
            var progressLogs = await _unitOfWork.Repository<ProgressLog, ProgressLog>().GetAllAsync();
            var existingProgress = progressLogs
                .FirstOrDefault(p => p.QuitPlanId == quitPlan.Id && p.LogDate.Date == today);

            if (existingProgress != null)
            {
                return new BaseResponseModel(StatusCodes.Status400BadRequest, "PROGRESS_EXISTS", "Bạn đã ghi nhận tiến trình hôm nay.");
            }
            
            
            // 5. Tạo ProgressLog mới, dùng adviceText làm Note (hoặc tuỳ ý)
            var progressLog = new ProgressLog
            {
                QuitPlanId = quitPlan.Id,
                SmokedToday = 0, // hoặc cho phép truyền vào từ UI
                Note = latestAdvice?.AdviceText ?? "Tự động ghi nhận từ lời khuyên.",
                LogDate =  DateTime.UtcNow,
                CreatedTime =  DateTime.UtcNow,
                LastUpdatedTime =  DateTime.UtcNow
            };

            await _unitOfWork.Repository<ProgressLog, Guid>().AddAsync(progressLog);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel(StatusCodes.Status200OK, "SUCCESS", "Ghi nhận tiến trình thành công.");
        }


        public async Task<BaseResponseModel> Update(ProgressLogsRequest request, Guid id)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            var repo = _unitOfWork.Repository<ProgressLog, Guid>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            _mapper.Map(request, entity);
            // Optionally update LogDate if you want to allow editing the date
            entity.LastUpdatedBy = userId;
            entity.LastUpdatedTime =  DateTime.UtcNow;
            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.CREATE_SUCCESS);
        }

        public async Task<BaseResponseModel> Delete(Guid id)
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            var repo = _unitOfWork.Repository<ProgressLog, Guid>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            entity.LastUpdatedBy = userId;
            entity.LastUpdatedTime =  DateTime.UtcNow;
            entity.DeletedBy = userId;
            entity.DeletedTime =  DateTime.UtcNow;

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.DELETE_SUCCESS);
        }

        public async Task<PaginatedList<ProgressLogsResponse>> getAllProgressLogs(PagingRequestModel model, ProgressLogsFillter fillter)
        {
            var culture = new CultureInfo("en-GB"); // "dd/MM/yyyy"
            DateTime? parsedLogDate = null;

            if (!string.IsNullOrEmpty(fillter.LogDate) &&
                DateTime.TryParseExact(fillter.LogDate, "dd/MM/yyyy", culture, DateTimeStyles.None, out var tempDate))
            {
                parsedLogDate = tempDate.Date;
            }

            var baseSpeci = new BaseSpecification<ProgressLog>(pl =>
                (string.IsNullOrEmpty(fillter.QuitPlanName) || pl.QuitPlan.Reason.Contains(fillter.QuitPlanName)) &&
                (!parsedLogDate.HasValue || (pl.LogDate.Date >= parsedLogDate.Value && pl.LogDate.Date <= DateTime.Now.Date)) &&
                (string.IsNullOrEmpty(fillter.Note) || pl.Note.Contains(fillter.Note))
            );

            var response = await _unitOfWork.Repository<ProgressLog, ProgressLog>().GetAllWithSpecAsync(baseSpeci);
            var result = _mapper.Map<List<ProgressLogsResponse>>(response);
            return PaginatedList<ProgressLogsResponse>.Create(result, model.PageNumber, model.PageSize);
        }

        public async Task<BaseResponseModel<ProgressLogsResponse>> getProgressLogsyId(Guid id)
        {
            var repo = _unitOfWork.Repository<ProgressLog, Guid>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            var result = _mapper.Map<ProgressLogsResponse>(entity);
            return new BaseResponseModel<ProgressLogsResponse>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.GET_SUCCESS);
        }
        private async Task<QuitPlan?> GetUserActiveOrLatestQuitPlan(Guid userId)
        {
            var quitPlans = await _unitOfWork.Repository<QuitPlan, QuitPlan>().GetAllAsync();
            var userQuitPlans = quitPlans.Where(q => q.UserId == userId);

            var quitPlan = userQuitPlans
                .FirstOrDefault(q => q.Status == QuitPlanStatus.Active)
                ?? userQuitPlans.OrderByDescending(q => q.CreatedTime).FirstOrDefault();

            // Tự động chuyển sang Completed nếu tới TargetDate
            if (quitPlan != null &&
                quitPlan.Status == QuitPlanStatus.Active &&
                 DateTime.UtcNow.Date >= quitPlan.TargetDate.Date)
            {
                quitPlan.Status = QuitPlanStatus.Completed;
                quitPlan.LastUpdatedTime =  DateTime.UtcNow;
                quitPlan.LastUpdatedBy = userId.ToString();

                await _unitOfWork.Repository<QuitPlan, Guid>().UpdateAsync(quitPlan);
                await _unitOfWork.SaveChangesAsync();
            }

            return quitPlan;
        }
    }
}
