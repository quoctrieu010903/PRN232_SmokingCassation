
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

namespace SmokingCessation.Application.Service.Implementations
{
    public class QuitPlanService : IQuitPlanService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IUserContext _userContext;

        public QuitPlanService(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userContext = userContext;
        }

        public async Task<BaseResponseModel> Create(QuitPlansRequest request)
        {
            var userID = _userContext.GetUserId();
            var currentUser = await _unitOfWork.Repository<ApplicationUser, Guid>().GetByIdAsync(Guid.Parse(userID));

        var baseSpeci = new BaseSpecification<Payment>(f => f.UserId == Guid.Parse(userID) && f.Status== PaymentStatus.Completed);
            var startDay = CoreHelper.SystemTimeNow;
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

        public async Task<BaseResponseModel> Delete(Guid id)
        {
            var currentUser = _userContext.GetUserId();
            var repo = _unitOfWork.Repository<QuitPlan, Guid>();
            var entity = await repo.GetByIdAsync(id);

            if (entity == null)
                return new BaseResponseModel(StatusCodes.Status404NotFound, "NOT_FOUND", "QuitPlan not found");

            entity.LastUpdatedBy = currentUser;
            entity.LastUpdatedTime = CoreHelper.SystemTimeNow;
            entity.DeletedBy = currentUser;
            entity.DeletedTime = CoreHelper.SystemTimeNow;


            await repo.UpdateAsync(entity);

            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, "SUCCESS", "Deleted successfully");

        }

        public async Task<PaginatedList<QuitPlanResponse>> getAllQuitPlan(PagingRequestModel model, QuitPlanFillter fillter, bool isCurrentUser)
        {
            Guid? currentUserId = null;
           
            if (isCurrentUser)
            {
                var userIdStr = _userContext.GetUserId();
                if (!Guid.TryParse(userIdStr, out var userId))
                    throw new Exception("Invalid or missing user id.");
                currentUserId = userId;
            }

            var baseSpeci = new BaseSpecification<QuitPlan>(mp =>
                !mp.DeletedTime.HasValue &&
                (!isCurrentUser || mp.UserId == currentUserId) &&
                (fillter == null || (
                    (fillter.Status == 0 || mp.Status == fillter.Status) &&
                    (string.IsNullOrEmpty(fillter.UserName) || mp.User.FullName.Contains(fillter.UserName)) &&
                    (fillter.StartDate == default || mp.StartDate.Date == fillter.StartDate.Date) &&
                    (fillter.TargetDate == default || mp.TargetDate.Date == fillter.TargetDate.Date)
                ))
            );

            var response = await _unitOfWork.Repository<QuitPlan, QuitPlan>().GetAllWithSpecAsync(baseSpeci);
            var result = _mapper.Map<List<QuitPlanResponse>>(response);
            return PaginatedList<QuitPlanResponse>.Create(result, model.PageNumber, model.PageSize);

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

        public async Task<BaseResponseModel> Update(QuitPlansRequest request, Guid id)
        {
            var currentUser = _userContext.GetUserId();
            var existedEntity = await _unitOfWork.Repository<QuitPlan,Guid>().GetByIdAsync(id);
            if(existedEntity == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);
            }
            existedEntity.LastUpdatedBy = currentUser;
            existedEntity.LastUpdatedTime = CoreHelper.SystemTimeNow;

            _mapper.Map(request, existedEntity);
            await _unitOfWork.Repository<QuitPlan,QuitPlan>().UpdateAsync(existedEntity);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK , ResponseCodeConstants.SUCCESS, "Update QuitPlan Successfully");

        }
    }
}
