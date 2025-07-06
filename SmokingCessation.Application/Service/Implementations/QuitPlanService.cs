using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SmokingCessation.Application.DTOs.Fillter;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Base;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.CustomExceptionss;
using SmokingCessation.Core.Response;
using SmokingCessation.Core.Utils;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Enums;
using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Domain.Specifications;
using System.Globalization;

namespace SmokingCessation.Application.Service.Implementations
{
    public class QuitPlanService : IQuitPlanService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAchievementService _userAchivement;
        private readonly ICoachAdviceLogService _coachAdviceLogService;
        public QuitPlanService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper , IUserAchievementService userAchievement, ICoachAdviceLogService coachAdviceLogService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _userAchivement = userAchievement;
            _coachAdviceLogService = coachAdviceLogService;
        }

        #region check lại LUỒNG
        /// <summary>
        /// Chekc luồng 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ErrorException"></exception>
        /// 
        /*
         public async Task<BaseResponseModel> Create(QuitPlansRequest request)
         {
             var userID = _userContext.GetUserId();
             var currentUser = await _unitOfWork.Repository<ApplicationUser, Guid>().GetByIdAsync(Guid.Parse(userID));


             var existingQuitPlanSpec = new BaseSpecification<QuitPlan>(
              x => x.UserId == Guid.Parse(userID) &&
             x.Status == QuitPlanStatus.Active &&
             !x.DeletedTime.HasValue
    );

             var existingQuitPlans = await _unitOfWork.Repository<QuitPlan, QuitPlan>().GetAllWithSpecAsync(existingQuitPlanSpec);

             if (existingQuitPlans.Any())
             {
                 throw new ErrorException(
                     StatusCodes.Status400BadRequest,
                     ResponseCodeConstants.BADREQUEST,
                     "Người dùng đã có kế hoạch bỏ thuốc đang hoạt động."
                 );
             }


             var baseSpeci = new BaseSpecification<Payment>(f => f.UserId == Guid.Parse(userID) && f.Status== PaymentStatus.Completed);
             var startDay =  DateTime.UtcNow;
             int targetDay = 0;
             var payments = await _unitOfWork.Repository<Payment, Payment>().GetAllWithSpecAsync(baseSpeci);
             var latestPaidPackage = payments
                 .OrderByDescending(x => x.CreatedTime)
                 .Select(x => new { x.PackageId, x.CreatedTime })
                 .FirstOrDefault();
             if (latestPaidPackage == null)
             {
                 throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Người dùng chưa thanh toán gói nào.");
             }
             // Lấy ra gói package đã thanh toán
             var package = await _unitOfWork.Repository<MembershipPackage, Guid>().GetByIdAsync(latestPaidPackage.PackageId);
             if (package == null)
             {
                 throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy gói membership.");
             }

             targetDay = package.DurationMonths > 0 ? package.DurationMonths * 30 : 30;
             var targetDate = startDay.AddDays(targetDay);

             var quitPlan = new QuitPlan
             {
                 Reason = request.Reason,
                 StartDate = startDay,
                 TargetDate = targetDate,
                 Status = QuitPlanStatus.Active,
                 UserId = Guid.Parse(userID),

                 CreatedBy = userID,
                 LastUpdatedBy = userID,
                 LastUpdatedTime = startDay,
                 CreatedTime = startDay,
             };
             await _unitOfWork.Repository<QuitPlan, Guid>().AddAsync(quitPlan);
             await _unitOfWork.SaveChangesAsync();
             return new BaseResponseModel(StatusCodes.Status200OK, "SUCCESS","Tạo data thành công");
         }
         */
        #endregion

        public async Task<BaseResponseModel> Create(QuitPlansRequest request)
        {
            var userID = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userGuid = Guid.Parse(userID);

            // 1. Kiểm tra xem đã có QuitPlan đang hoạt động chưa
            var existingQuitPlanSpec = new BaseSpecification<QuitPlan>(
                x => x.UserId == userGuid &&
                     x.Status == QuitPlanStatus.Active &&
                     !x.DeletedTime.HasValue);

            var existingQuitPlans = await _unitOfWork.Repository<QuitPlan, QuitPlan>()
                .GetAllWithSpecAsync(existingQuitPlanSpec);

            if (existingQuitPlans.Any())
            {
                throw new ErrorException(
                    StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.BADREQUEST,
                    "Người dùng đã có kế hoạch bỏ thuốc đang hoạt động."
                );
            }

            // 2. Kiểm tra xem người dùng có gói user package còn hiệu lực không
            var today =  DateTime.UtcNow;
            var userPackageSpec = new BaseSpecification<UserPackage>(
                x => x.UserId == userGuid &&
                     x.StartDate <= today &&
                     x.EndDate >= today &&
                     x.IsActive);

            var validUserPackages = await _unitOfWork.Repository<UserPackage, UserPackage>()
                .GetAllWithSpecAsync(userPackageSpec);

            if (!validUserPackages.Any())
            {
                throw new ErrorException(
                    StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.BADREQUEST,
                    "Người dùng chưa đăng ký gói thành viên hợp lệ để tạo kế hoạch bỏ thuốc."
                );
            }

            // 3. Kiểm tra gói đã thanh toán (Payment)
            var baseSpeci = new BaseSpecification<Payment>(
                f => f.UserId == userGuid && f.Status == PaymentStatus.Success);

            var payments = await _unitOfWork.Repository<Payment, Payment>()
                .GetAllWithSpecAsync(baseSpeci);

            var latestPaidPackage = payments
                .OrderByDescending(x => x.CreatedTime)
                .Select(x => new { x.PackageId, x.CreatedTime })
                .FirstOrDefault();

            if (latestPaidPackage == null)
            {
                throw new ErrorException(
                    StatusCodes.Status404NotFound,
                    ResponseCodeConstants.NOT_FOUND,
                    "Người dùng chưa thanh toán gói nào."
                );
            }

            var package = await _unitOfWork.Repository<MembershipPackage, Guid>()
                .GetByIdAsync(latestPaidPackage.PackageId);

            if (package == null)
            {
                throw new ErrorException(
                    StatusCodes.Status404NotFound,
                    ResponseCodeConstants.NOT_FOUND,
                    "Không tìm thấy gói membership."
                );
            }

            // 4. Tính toán ngày target dựa trên gói
            int targetDay = package.DurationMonths > 0 ? package.DurationMonths * 30 : 30;
            var startDay =  DateTime.UtcNow;
            var targetDate = startDay.AddDays(targetDay);

            // 5. Tạo QuitPlan
            var quitPlan = new QuitPlan
            {
                Reason = request.Reason,
                StartDate = startDay,
                TargetDate = targetDate,
                Status = QuitPlanStatus.Active,
                CigarettesPerDayBeforeQuit = request.CigarettesPerDayBeforeQuit,
                YearsSmokingBeforeQuit = request.YearsSmokingBeforeQuit,
                UserId = userGuid,
                CreatedBy = userID,
                LastUpdatedBy = userID,
                LastUpdatedTime = startDay,
                CreatedTime = startDay,
            };

            await _unitOfWork.Repository<QuitPlan, QuitPlan>().AddAsync(quitPlan);
            await _unitOfWork.SaveChangesAsync();

            await _userAchivement.AssignAchievementsIfEligibleAsync(userGuid);
            await _coachAdviceLogService.CreateAdviceLogAsync(userGuid);



            return new BaseResponseModel(
                StatusCodes.Status200OK,
                "SUCCESS",
                "Tạo kế hoạch bỏ thuốc thành công."
            );
        }

        public async Task<BaseResponseModel> Delete(Guid id)
        {
            var currentUser = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            var repo = _unitOfWork.Repository<QuitPlan, Guid>();
            var entity = await repo.GetByIdAsync(id);

            if (entity == null)
                return new BaseResponseModel(StatusCodes.Status404NotFound, "NOT_FOUND", "QuitPlan not found");
          
            entity.LastUpdatedBy = currentUser;
            entity.LastUpdatedTime =  DateTime.UtcNow;
     


            await repo.UpdateAsync(entity);

            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, "SUCCESS", "Change Status  successfully");

        }
        

        public async Task<PaginatedList<QuitPlanResponse>> getAllQuitPlan(PagingRequestModel model, QuitPlanFillter fillter, bool isCurrentUser)
        {
            Guid? currentUserId = null;
           
            if (isCurrentUser)
            {
                var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
                if (!Guid.TryParse(userIdStr, out var userId))
                    throw new ErrorException(StatusCodes.Status400BadRequest,ResponseCodeConstants.NOT_FOUND,"Invalid or missing user id.");
                currentUserId = userId;
            }
            var culture = new CultureInfo("en-GB"); // dd/MM/yyyy
            DateTime? parsedStartDate = null;
            DateTime? parsedTargetDate = null;

            if (!string.IsNullOrEmpty(fillter.StartDate))
                parsedStartDate = DateTime.SpecifyKind(DateTime.ParseExact(fillter.StartDate, "dd/MM/yyyy", culture), DateTimeKind.Utc);

            if (!string.IsNullOrEmpty(fillter.TargetDate))
                parsedTargetDate = DateTime.SpecifyKind(DateTime.ParseExact(fillter.TargetDate, "dd/MM/yyyy", culture), DateTimeKind.Utc);


            var baseSpeci = new BaseSpecification<QuitPlan>(mp =>
                !mp.DeletedTime.HasValue &&
                (!isCurrentUser || mp.UserId == currentUserId) &&
                (fillter == null || (
                    (fillter.Status == 0 || mp.Status == fillter.Status) &&
                    (string.IsNullOrEmpty(fillter.UserName) || mp.User.FullName.Contains(fillter.UserName)) &&
                           (fillter.StartDate == default || mp.StartDate.Date == parsedStartDate) &&
                            (fillter.TargetDate == default || mp.TargetDate.Date == parsedTargetDate)
                ))
            ); 

            var response = await _unitOfWork.Repository<QuitPlan, QuitPlan>().GetAllWithSpecWithInclueAsync(baseSpeci, true,p=>p.MembershipPackage , p=> p.AdviceLogs);
            var result = _mapper.Map<List<QuitPlanResponse>>(response);
            return PaginatedList<QuitPlanResponse>.Create(result, model.PageNumber, model.PageSize);

        }

        public async Task<BaseResponseModel<QuitPlanResponse?>> GetQuitPlanAsync(Guid? userId = null)
        {
            var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value);
           
           
                
            var currentUser = _httpContextAccessor.HttpContext?.User;
            bool isAdmin = currentUser != null && currentUser.IsInRole(UserRoles.Admin);

            if (!isAdmin)
            {
                 throw new ErrorException(
                StatusCodes.Status403Forbidden,
                ResponseCodeConstants.FORBIDDEN,
                "Bạn không có quyền truy cập kế hoạch của người khác.");
            }

            // Truy xuất kế hoạch active của user, bao gồm thông tin Package
            var quitPlanList = await _unitOfWork.Repository<QuitPlan, Guid>()
                    .GetAllWithIncludeAsync(
                         true,
                         p => p.MembershipPackage
                    );

            // 2. Tìm kế hoạch đang active của user hiện tại
            var plan = quitPlanList
                .FirstOrDefault(q => q.UserId == currentUserId && q.Status == QuitPlanStatus.Active);

            if (plan == null)
            {
                throw new ErrorException(
                    StatusCodes.Status404NotFound,
                    ResponseCodeConstants.NOT_FOUND,
                    MessageConstants.NOT_FOUND);
            }

            var baseSpeci = new BaseSpecification<ProgressLog>(p => Guid.Parse(p.CreatedBy) == currentUserId && p.LogDate >= plan.StartDate.Date);
            // Lấy progress log để tính ngày không hút
            var logs = await _unitOfWork.Repository<ProgressLog, Guid>()
                .GetAllWithSpecAsync(baseSpeci);

            var smokeFreeDays = logs.Count(l => l.SmokedToday == 0);
            var health = CalculateHealthImpact(smokeFreeDays);

            var response = new QuitPlanResponse
            {
                Id = plan.Id,
                Reason = plan.Reason,
                StartDate = plan.StartDate,
                TargetDate = plan.TargetDate,
                CigarettesPerDayBeforeQuit = plan.CigarettesPerDayBeforeQuit,
                YearsSmokingBeforeQuit = plan.YearsSmokingBeforeQuit,
                Status = plan.Status.ToString(),
                UserId = plan.UserId,
                PackageId = plan.MembershipPackage.Id,
                PackageName = plan.MembershipPackage?.Name ?? "Không rõ",
                SmokeFreeDays = smokeFreeDays,
                HealthImpact = health
            };

            return new BaseResponseModel<QuitPlanResponse>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, response);
        }



        public async Task<BaseResponseModel<QuitPlanResponse>> getQuitPlanById(Guid id)
        {
            var entity = await _unitOfWork.Repository<QuitPlan, Guid>().GetByIdAsync(id);
            if (entity == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);
            }
            var result = _mapper.Map<QuitPlanResponse>(entity);

            return new BaseResponseModel<QuitPlanResponse>(StatusCodes.Status200OK,ResponseCodeConstants.SUCCESS, result);
           
           
        }

        public async Task<BaseResponseModel> UpdateStatus( Guid id, QuitPlanStatus request)
        {
            var currentUser = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existedEntity = await _unitOfWork.Repository<QuitPlan,Guid>().GetByIdAsync(id);
            if(existedEntity == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);
            }
            // Optional: Không cho chuyển Completed về trạng thái khác
            if (existedEntity.Status == QuitPlanStatus.Completed)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Kế hoạch đã hoàn thành không thể cập nhật.");
            }
            // Không cho phép cập nhật thủ công sang Completed
            if (request == QuitPlanStatus.Completed)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Không thể tự cập nhật sang trạng thái hoàn thành.");
            }


            existedEntity.LastUpdatedBy = currentUser;
            existedEntity.LastUpdatedTime =  DateTime.UtcNow;
            existedEntity.Status = request;
            

            _mapper.Map(request, existedEntity);
            await _unitOfWork.Repository<QuitPlan,QuitPlan>().UpdateAsync(existedEntity);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK , ResponseCodeConstants.SUCCESS, "Update QuitPlan Successfully");

        }

        private HealthImpactProgress CalculateHealthImpact(int smokeFreeDays)
        {
            var cancer = Math.Min(100.0 * smokeFreeDays / 7300.0, 50.0);  // 20 năm = max 50%
            var heart = Math.Min(100.0 * smokeFreeDays / 3650.0, 95.0);   // 10 năm = max 95%

            return new HealthImpactProgress
            {
                CancerRiskReductionPercent = Math.Round(cancer, 1),
                HeartRiskReductionPercent = Math.Round(heart, 1),
                Summary = $"Bạn đã giảm {Math.Round(cancer, 1)}% nguy cơ ung thư và {Math.Round(heart, 1)}% nguy cơ bệnh tim."
            };
        }
    }
}
