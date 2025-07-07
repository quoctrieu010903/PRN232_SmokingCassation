using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class UserPackageService : IUserPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IVNPayService _vnpayService;

        public UserPackageService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IVNPayService vnpayService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _vnpayService = vnpayService;
        }

        public async Task<BaseResponseModel<UserPackageResponse>> CancelCurrentPackage()
        {
           
            var id = _httpContextAccessor.HttpContext?.User?.FindFirstValue("UserId");
            Console.WriteLine("id"+id);
            if (id == null)
                return new BaseResponseModel<UserPackageResponse>(StatusCodes.Status403Forbidden,
                    ResponseCodeConstants.UNAUTHORIZED, MessageConstants.FORBIDDEN);

            var userId = Guid.Parse(id);

            var now = DateTime.UtcNow;
            var userpackageRepo = _unitOfWork.Repository<UserPackage, Guid>();

            var current = (await userpackageRepo.GetAllWithSpecAsync(
                    new BaseSpecification<UserPackage>(um => um.UserId == userId && um.IsActive && um.EndDate > now)))
                .FirstOrDefault();

            if (current == null)
                return new BaseResponseModel<UserPackageResponse>(StatusCodes.Status404NotFound,
                    ResponseCodeConstants.NO_ACTIVE_MEMBERSHIP, MessageConstants.NO_ACTIVE_MEMBERSHIP);

            current.IsActive = false;
            current.EndDate = now;
            current.CancelledDate = now;
            await userpackageRepo.UpdateAsync(current);
            await _unitOfWork.SaveChangesAsync();

            // Load navigation properties if needed
            var packageRepo = _unitOfWork.Repository<MembershipPackage, Guid>();
            current.Package = await packageRepo.GetByIdAsync(current.PackageId);
            var response = _mapper.Map<UserPackageResponse>(current);
            return new BaseResponseModel<UserPackageResponse>(StatusCodes.Status200OK,
                ResponseCodeConstants.CANCEL_PACKAGE_SUCCESS, response, null, MessageConstants.CANCEL_PACKAGE_SUCCESS);
        }

        public async Task<BaseResponseModel<string>> DeleteUserPackageAsync(Guid userPackageId)
        {
            var userPackageRepo = _unitOfWork.Repository<UserPackage, Guid>();

            var userPackage = await userPackageRepo.GetByIdAsync(userPackageId);
            if (userPackage == null)
            {
                throw new ErrorException(
                    StatusCodes.Status404NotFound,
                    ResponseCodeConstants.NOT_FOUND,
                    "Không tìm thấy gói của người dùng.");
            }

            await _unitOfWork.Repository<UserPackage ,UserPackage>().DeleteAsync(userPackage); // Xoá cứng
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel<string>(
                StatusCodes.Status200OK,
                ResponseCodeConstants.SUCCESS,
                null,
                null,
                "Xoá gói người dùng thành công.");
        }



        public async Task<BaseResponseModel<UserPackageResponse>> GetCurrentPackage()
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value);

            var now = DateTime.UtcNow;
            var userpackageRepo = _unitOfWork.Repository<UserPackage, Guid>();

            var current = (await userpackageRepo.GetAllWithSpecWithInclueAsync(
                    new BaseSpecification<UserPackage>(um => um.UserId == userId && um.IsActive && um.EndDate > now),
                    true, f => f.User, f => f.Package))
                .FirstOrDefault();

            if (current == null)
                return new BaseResponseModel<UserPackageResponse>(StatusCodes.Status404NotFound,
                    ResponseCodeConstants.NO_ACTIVE_MEMBERSHIP, MessageConstants.NO_ACTIVE_MEMBERSHIP);

            // Load navigation properties if needed
            var packageRepo = _unitOfWork.Repository<MembershipPackage, Guid>();
            current.Package = await packageRepo.GetByIdAsync(current.PackageId);
            var response = _mapper.Map<UserPackageResponse>(current);
            return new BaseResponseModel<UserPackageResponse>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS,
                response);
        }

        public async Task<BaseResponseModel<UserPackageResponse>> GetPackageById(Guid id)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value);

            var userpackageRepo = _unitOfWork.Repository<UserPackage, Guid>();

            var package = await userpackageRepo.GetByIdAsync(id);
            if (package == null || package.UserId != userId)
                return new BaseResponseModel<UserPackageResponse>(StatusCodes.Status404NotFound,
                    ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            // Load navigation properties if needed
            var packageRepo = _unitOfWork.Repository<MembershipPackage, Guid>();
            package.Package = await packageRepo.GetByIdAsync(package.PackageId);

            var response = _mapper.Map<UserPackageResponse>(package);
            return new BaseResponseModel<UserPackageResponse>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS,
                response);
        }


        public async Task<BaseResponseModel<List<UserPackageResponse>>> GetPackageHistory()
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value);

            var userpackageRepo = _unitOfWork.Repository<UserPackage, Guid>();

            var history = (await userpackageRepo.GetAllWithSpecAsync(
                    new BaseSpecification<UserPackage>(um => um.UserId == userId)))
                .OrderByDescending(x => x.StartDate)
                .ToList();

            // Load package info for each item if needed
            var packageRepo = _unitOfWork.Repository<MembershipPackage, Guid>();
            foreach (var item in history)
            {
                item.Package = await packageRepo.GetByIdAsync(item.PackageId);
            }

            var response = _mapper.Map<List<UserPackageResponse>>(history);
            return new BaseResponseModel<List<UserPackageResponse>>(StatusCodes.Status200OK,
                ResponseCodeConstants.SUCCESS, response);
        }

        public async Task<BaseResponseModel<VNPayReturnLink>> RegisterPackage(UserPackageRequest request)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value);

            var now = DateTime.UtcNow;


            // Check for existing active package  
            var userpackageRepo = _unitOfWork.Repository<UserPackage, Guid>();
            var hasActive = (await userpackageRepo.GetAllWithSpecAsync(
                    new BaseSpecification<UserPackage>(um => um.UserId == userId && um.IsActive && um.EndDate > now)))
                .Any();

            if (hasActive)

                throw new ErrorException(StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.ACTIVE_PACKAGE_EXISTS,
                    MessageConstants.ACTIVE_PACKAGE_EXISTS);


            // Get package info  
            var packageRepo = _unitOfWork.Repository<MembershipPackage, Guid>();
            var package = await packageRepo.GetByIdAsync(request.MembershipPackageId);
            if (package == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.PACKAGE_NOT_FOUND,
                    MessageConstants.PACKAGE_NOT_FOUND);
            }

            string paymentURL = "";
            try
            {
                //get paymentURL
                var paymentCreateDTO = new PaymentCreateRequest
                {
                    UserId = userId,
                    BookingId = package.Id,
                    MoneyUnit = "VND",
                    PaymentContent = $"Payment For {package.Name}",
                    TotalAmount = Convert.ToSingle(package.Price),
                };
                paymentURL = await _vnpayService.GeneratePaymentUrlAsync(paymentCreateDTO);
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }


            return new BaseResponseModel<VNPayReturnLink>(StatusCodes.Status200OK,
                ResponseCodeConstants.REGISTER_PACKAGE_SUCCESS, paymentURL);
        }

        public async Task<BaseResponseModel<string>> DeleteUserPackageAsyncs(Guid userPackageId)
        {
            var userPackageRepo = _unitOfWork.Repository<UserPackage, Guid>();
            var quitPlanRepo = _unitOfWork.Repository<QuitPlan, Guid>(); // Cần repository cho QuitPlan

            var userPackage = await userPackageRepo.GetByIdAsync(userPackageId);
            if (userPackage == null)
            {
                throw new ErrorException(
                    StatusCodes.Status404NotFound,
                    ResponseCodeConstants.NOT_FOUND,
                    "Không tìm thấy gói của người dùng.");
            }

            var userId = userPackage.UserId; // Lấy UserId từ UserPackage
            var packageIdOfDeletedUserPackage = userPackage.PackageId; // Lấy MembershipPackageId của gói đang bị xóa

        var quiplanRepo = _unitOfWork.Repository<QuitPlan, Guid>();
            var quitPlansToRemove = await quiplanRepo.GetAllWithSpecAsync(
                new BaseSpecification<QuitPlan>(qp => qp.UserId == userId && qp.MembershipPackageId == packageIdOfDeletedUserPackage), true);
            if (quitPlansToRemove.Any())
            {
                Guid[] quitPlanIdsToDelete = quitPlansToRemove.Select(qp => qp.Id).ToArray();

                await quitPlanRepo.DeleteRangeAsync(quitPlanIdsToDelete); 

            }
         
            await userPackageRepo.DeleteAsync(userPackageId); 

       
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel<string>(
                StatusCodes.Status200OK,
                ResponseCodeConstants.SUCCESS,
                null,
                null,
                "Xóa gói người dùng và các kế hoạch cai thuốc liên quan thành công.");
        }
    }
}