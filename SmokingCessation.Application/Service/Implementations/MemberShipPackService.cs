

using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SmokingCessation.Application.DTOs.Fillter;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.Response;
using SmokingCessation.Core.Utils;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Domain.Specifications;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace SmokingCessation.Application.Service.Implementations
{
    public class MemberShipPackService : IMemberShipPackage
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;

        public MemberShipPackService(IUnitOfWork unitOfWork , IMapper mapper , IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userContext =userContext;
        }

        public async Task<BaseResponseModel> Create(MemberShipPackageRequest request)
        {
            var currentUser = _userContext.GetUserId();

            var entity = _mapper.Map<MembershipPackage>(request);

            entity.CreatedBy = currentUser;
            entity.CreatedTime =  DateTime.UtcNow;
            entity.LastUpdatedBy = currentUser;
            entity.LastUpdatedTime =  DateTime.UtcNow;

            await _unitOfWork.Repository<MembershipPackage, bool>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, entity);

            
        }

        public async Task<BaseResponseModel> Delete(Guid id)
        {
            var currentUser = _userContext.GetUserId();
            var repo = _unitOfWork.Repository<MembershipPackage, Guid>();
            var entity = await repo.GetByIdAsync(id);

            if (entity == null)
                return new BaseResponseModel(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND,MessageConstants.NOT_FOUND);

            entity.LastUpdatedBy = currentUser;
            entity.LastUpdatedTime =  DateTime.UtcNow;
            entity.DeletedBy = currentUser;
            entity.DeletedTime =  DateTime.UtcNow;
            

            await repo.UpdateAsync(entity);

            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.DELETE_SUCCESS);

        }

        public async Task<PaginatedList<MemberShipPackageResponse>> getAllPackage(PagingRequestModel model, MemberShipPackageFillter fillter )
        {
            var baseSpeci = new BaseSpecification<MembershipPackage>(mp =>
                !mp.DeletedTime.HasValue &&
                (string.IsNullOrEmpty(fillter.Name) || mp.Name.Contains(fillter.Name)) &&
                (string.IsNullOrEmpty(fillter.Description) || mp.Description.Contains(fillter.Description)) &&
                (!fillter.Type.HasValue || mp.Type == fillter.Type.Value)
    );

            var response = await _unitOfWork.Repository<MembershipPackage, MembershipPackage>().GetAllWithSpecWithInclueAsync(baseSpeci, true );
            var result = _mapper.Map<List<MemberShipPackageResponse>>(response);
            
            return PaginatedList<MemberShipPackageResponse>.Create(result, model.PageNumber,
                model.PageSize);
        }

        public async Task<BaseResponseModel> getPackageById(Guid id)
        {
            var repo = _unitOfWork.Repository<MembershipPackage, Guid>();
            var entity = await repo.GetByIdAsync(id);

            if (entity == null || entity.DeletedTime.HasValue)
                return new BaseResponseModel(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            var result = _mapper.Map<MemberShipPackageResponse>(entity);
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, result);

        }

        public async Task<BaseResponseModel> Update(MemberShipPackageRequest request, Guid id)
        {

            var currentUser = _userContext.GetUserId();

            var repo = _unitOfWork.Repository<MembershipPackage, Guid>();

            var entity = await repo.GetByIdAsync(id);
            if (entity == null)
                return new BaseResponseModel(404, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            entity.LastUpdatedBy = currentUser;
            entity.LastUpdatedTime =  DateTime.UtcNow;
            _mapper.Map(request, entity);

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, entity);

        }
    }
}
