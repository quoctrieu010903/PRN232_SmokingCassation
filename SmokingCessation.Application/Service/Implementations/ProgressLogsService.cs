
using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmokingCessation.Application.DTOs.Fillter;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.CustomExceptionss;
using SmokingCessation.Core.Response;
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
            entity.Status = ProgressLogStatus.Pending;
            entity.LogDate = DateTime.UtcNow;
            entity.CreatedTime = DateTime.UtcNow;
            entity.LastUpdatedTime = DateTime.UtcNow;

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
            var today = DateTime.UtcNow.Date;
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
                LogDate = DateTime.UtcNow,
                CreatedTime = DateTime.UtcNow,
                LastUpdatedTime = DateTime.UtcNow
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
            {
                throw new ErrorException(
                    StatusCodes.Status404NotFound,
                    ResponseCodeConstants.NOT_FOUND,
                    MessageConstants.NOT_FOUND
                );
            }

            var today = DateTime.UtcNow.Date;

            // ❌ Chỉ cho phép cập nhật log đúng ngày hiện tại
            if (entity.LogDate.Date != today)
            {
                throw new ErrorException(
                    StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.VALIDATION_ERROR,
                    "Chỉ được phép cập nhật nhật ký của ngày hôm nay."
                );
            }

            // ❌ Chỉ cho phép cập nhật nếu trạng thái là Pending hoặc Failed
            if (entity.Status != ProgressLogStatus.Pending && entity.Status != ProgressLogStatus.Failed)
            {
                throw new ErrorException(
                    StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.VALIDATION_ERROR,
                    "Chỉ được phép cập nhật nhật ký khi trạng thái là Pending hoặc Failed."
                );
            }

            _mapper.Map(request, entity);
            entity.LastUpdatedBy = userId;
            entity.LastUpdatedTime = DateTime.UtcNow;

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel(
                StatusCodes.Status200OK,
                ResponseCodeConstants.SUCCESS,
                MessageConstants.UPDATE_SUCCESS
            );
        }

        public async Task<BaseResponseModel> Delete(Guid id)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            var repo = _unitOfWork.Repository<ProgressLog, Guid>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            entity.LastUpdatedBy = userId;
            entity.LastUpdatedTime = DateTime.UtcNow;
            entity.DeletedBy = userId;
            entity.DeletedTime = DateTime.UtcNow;

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.DELETE_SUCCESS);
        }

        public async Task<PaginatedList<ProgressLogsResponse>> getAllProgressLogs(PagingRequestModel model, ProgressLogsFillter fillter)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
        
            var culture = new CultureInfo("en-GB"); // "dd/MM/yyyy"
            DateTime todayUtc = DateTime.UtcNow.Date;
            DateTime? parsedLogDate = null;

            if (!string.IsNullOrEmpty(fillter.LogDate))
            {
                // Parse ngày và đảm bảo nó là UTC
                if (DateTime.TryParseExact(fillter.LogDate, "dd/MM/yyyy", culture, DateTimeStyles.None, out DateTime tempDate))
                
                    parsedLogDate = DateTime.SpecifyKind(tempDate.Date, DateTimeKind.Utc); // Chỉ lấy phần ngày và đặt 
            }
           
            var baseSpeci = new BaseSpecification<ProgressLog>(pl =>
                 pl.CreatedBy == userId &&
                (string.IsNullOrEmpty(fillter.QuitPlanName) || pl.QuitPlan.Reason.Contains(fillter.QuitPlanName)) &&
                (
                    (!parsedLogDate.HasValue ) ||  // nếu không nhập thì lấy từ hôm nay trở đi
                    (parsedLogDate.HasValue && pl.LogDate.Date >= parsedLogDate.Value) // nếu có nhập thì lấy từ ngày nhập
                ) &&
                (string.IsNullOrEmpty(fillter.Note) || pl.Note.Contains(fillter.Note)) && 
                (fillter.status == null || pl.Status == fillter.status)
            );
            


            var response = await _unitOfWork.Repository<ProgressLog, ProgressLog>().GetAllWithSpecWithInclueAsync(baseSpeci, true,p=> p.QuitPlan);
            var result = _mapper.Map<List<ProgressLogsResponse>>(response).OrderBy(x => x.LogDate).ToList();
            return PaginatedList<ProgressLogsResponse>.Create(result, model.PageNumber, model.PageSize);
        }



       
        public async Task<BaseResponseModel<ProgressLogsResponse>> getProgressLogsyId(Guid id)
        {
            var repo = _unitOfWork.Repository<ProgressLog, Guid>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            var result = _mapper.Map<ProgressLogsResponse>(entity);
            return new BaseResponseModel<ProgressLogsResponse>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, result);
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
                quitPlan.LastUpdatedTime = DateTime.UtcNow;
                quitPlan.LastUpdatedBy = userId.ToString();

                await _unitOfWork.Repository<QuitPlan, Guid>().UpdateAsync(quitPlan);
                await _unitOfWork.SaveChangesAsync();
            }

            return quitPlan;
        }

        public async Task<BaseResponseModel> CreateProgesslogFromQuitPlain(Guid Userid, ProgressLogsRequest request, Guid QuiPlanId)
        {
            var quitPlan = await _unitOfWork.Repository<QuitPlan, QuitPlan>().GetWithSpecAsync(new BaseSpecification<QuitPlan>(q => q.UserId == Userid && q.Id == QuiPlanId), true);


            if (quitPlan == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);
            }

            var startDate = quitPlan.StartDate.Date;
            var endDate = startDate.AddDays(6); // ❗ Tạo trước 7 ngày (tức từ StartDate đến StartDate + 6)
            var today = DateTime.UtcNow.Date;

            var existingDates = (quitPlan.ProgressLogs ?? new List<ProgressLog>())
                                 .Select(pl => pl.LogDate.Date)
                                 .ToHashSet();
            var newLogs = new List<ProgressLog>();

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (existingDates.Contains(date)) continue;

                var status = date < today
                    ? ProgressLogStatus.Failed
                    : ProgressLogStatus.Pending;

                newLogs.Add(new ProgressLog
                {

                    Id = Guid.NewGuid(),
                    QuitPlanId = quitPlan.Id,
                    LogDate = date,
                    Status = status,
                    SmokedToday = 0,
                    Note = string.Empty,
                    CreatedBy = Userid.ToString(),
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedBy = Userid.ToString(),
                    LastUpdatedTime = DateTime.UtcNow


                });
            }

            if (newLogs.Any())
            {
                await _unitOfWork.Repository<ProgressLog, ProgressLog>().AddRangeAsync(newLogs);
                await _unitOfWork.SaveChangesAsync();
            }

            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.CREATE_SUCCESS);
        }

        public async Task<BaseResponseModel> UpdateStatusProgressLog(ProgressLogStatus request, Guid id)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;

            var progressLog = await _unitOfWork.Repository<ProgressLog, Guid>().GetByIdAsync(id);
            if (progressLog == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);
            }
            if (progressLog.CreatedBy != userId)
            {
                throw new ErrorException(
                    StatusCodes.Status403Forbidden,
                    ResponseCodeConstants.FORBIDDEN,
                    "Bạn không có quyền chỉnh sửa nhật ký này."
                );
            }
            var today = DateTime.UtcNow.Date;

            // ❌ Chỉ được phép cập nhật status nếu log đúng ngày hôm nay
            if (progressLog.LogDate.Date != today)
            {
                throw new ErrorException(
                    StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.VALIDATION_ERROR,
                    "Chỉ được phép cập nhật trạng thái của log ngày hôm nay."
                );
            }

            if (progressLog.Status != ProgressLogStatus.Pending)
            {
                throw new ErrorException(
                    StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.VALIDATION_ERROR,
                    "Chỉ được phép cập nhật khi trạng thái là Pending hoặc Failed."
                );
            }

            if (progressLog.Status == request)
            {
                throw new ErrorException(
                    StatusCodes.Status200OK,
                    ResponseCodeConstants.NO_CHANGE,
                    "Trạng thái không thay đổi."
                );
            }

            progressLog.Status = request;
            progressLog.LastUpdatedBy = userId;
            progressLog.LastUpdatedTime = DateTime.UtcNow;

            await _unitOfWork.Repository<ProgressLog, Guid>().UpdateAsync(progressLog);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(
                   StatusCodes.Status200OK,
                   ResponseCodeConstants.SUCCESS,
                   "Cập nhật trạng thái thành công."
                     );
        }
    }
}
