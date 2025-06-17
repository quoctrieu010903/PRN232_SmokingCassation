using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SmokingCessation.Application.DTOs.Request;
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
        private readonly IUserContext _context;

        public UserPackageService(IUnitOfWork unitOfWork, IMapper mapper, IUserContext Context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = Context;
        }

        public async Task<BaseResponseModel> CancelCurrentPackage()
        {
            var userId = Guid.Parse(_context.GetUserId());
            var now = CoreHelper.SystemTimeNow;
            var userpackageRepo = _unitOfWork.Repository<UserPackage, Guid>();

            var current = (await userpackageRepo.GetAllWithSpecAsync(
                new BaseSpecification<UserPackage>(um => um.UserId == userId && um.IsActive && um.EndDate > now)))
                .FirstOrDefault();

            if (current == null)
                return new BaseResponseModel(StatusCodes.Status404NotFound, ResponseCodeConstants.NO_ACTIVE_MEMBERSHIP, MessageConstants.NO_ACTIVE_MEMBERSHIP);

            current.IsActive = false;
            current.EndDate = now;
            current.CancelledDate = now;
            await userpackageRepo.UpdateAsync(current);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.CANCEL_PACKAGE_SUCCESS, MessageConstants.CANCEL_PACKAGE_SUCCESS);


        }

        public async Task<BaseResponseModel> GetCurrentPackage()
        {
            var userId = Guid.Parse(_context.GetUserId());
            var now = CoreHelper.SystemTimeNow;
            var userpackageRepo = _unitOfWork.Repository<UserPackage, Guid>();

            var current = (await userpackageRepo.GetAllWithSpecAsync(
                new BaseSpecification<UserPackage>(um => um.UserId == userId && um.IsActive && um.EndDate > now)))
                .FirstOrDefault();

            if (current == null)
                return new BaseResponseModel(StatusCodes.Status404NotFound, ResponseCodeConstants.NO_ACTIVE_MEMBERSHIP, MessageConstants.NO_ACTIVE_MEMBERSHIP);

            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, current);

        }

        public async Task<BaseResponseModel> GetPackageById(Guid id)
        {
            var userId = Guid.Parse(_context.GetUserId());
            var userpackageRepo = _unitOfWork.Repository<UserPackage, Guid>();

            var package = await userpackageRepo.GetByIdAsync(id);
            if (package == null || package.UserId != userId)
                return new BaseResponseModel(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, package);

        }

        public async Task<BaseResponseModel> GetPackageHistory()
        {
            var userId = Guid.Parse(_context.GetUserId());
            var userpackageRepo = _unitOfWork.Repository<UserPackage, Guid>();

            var history = await userpackageRepo.GetAllWithSpecAsync(
                new BaseSpecification<UserPackage>(um => um.UserId == userId));

            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, history.OrderByDescending(x => x.StartDate).ToList());

        }

        public async Task<BaseResponseModel> RegisterPackage(UserPackageRequest request)
        {
            var userId = Guid.Parse(_context.GetUserId());
            var now = CoreHelper.SystemTimeNow;

            // Check for completed payment for this user  
            var paymentRepo = _unitOfWork.Repository<Payment, Guid>();
            var hasPaid = (await paymentRepo.GetAllWithSpecAsync(
                new BaseSpecification<Payment>(p =>
                    p.UserId == userId &&
                    p.PackageId == request.MembershipPackageId &&
                    p.Status == PaymentStatus.Completed)))
                .Any();

            if (!hasPaid)
            
                throw new ErrorException(StatusCodes.Status403Forbidden,
                                 ResponseCodeConstants.PAYMENT_REQUIRED,
                                 MessageConstants.PAYMENT_REQUIRED
                 );
            

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
                return new BaseResponseModel(StatusCodes.Status404NotFound, ResponseCodeConstants.PACKAGE_NOT_FOUND, MessageConstants.PACKAGE_NOT_FOUND);
            }

            var entity = new UserPackage
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PackageId = request.MembershipPackageId,
                StartDate = now,
                EndDate = now.AddMonths(package.DurationMonths),
                IsActive = true
            };

            await userpackageRepo.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.REGISTER_PACKAGE_SUCCESS, MessageConstants.REGISTER_PACKAGE_SUCCESS);
        }
    }
}
