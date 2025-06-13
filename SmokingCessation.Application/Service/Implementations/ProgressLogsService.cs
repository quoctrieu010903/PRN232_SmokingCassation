
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SmokingCessation.Application.DTOs.Fillter;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.CustomExceptionss;
using SmokingCessation.Core.Response;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Domain.Specifications;

namespace SmokingCessation.Application.Service.Implementations
{
    public class ProgressLogsService : IProgressLogsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;

        public ProgressLogsService(IUnitOfWork unitOfWork, IMapper mapper, IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userContext = userContext;
        }

        public async Task<BaseResponseModel> Create(ProgressLogsRequest request)
        {
            var quitPlan = await _unitOfWork.Repository<QuitPlan, Guid>().GetByIdAsync(request.QuitPlanId);
            if (quitPlan == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);
            }
            var entity = _mapper.Map<ProgressLog>(request);
            entity.LogDate = DateTimeOffset.UtcNow;

            await _unitOfWork.Repository<ProgressLog, Guid>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.CREATE_SUCCESS);
        }

        public async Task<BaseResponseModel> Update(ProgressLogsRequest request, Guid id)
        {
            var repo = _unitOfWork.Repository<ProgressLog, Guid>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            _mapper.Map(request, entity);
            // Optionally update LogDate if you want to allow editing the date

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.CREATE_SUCCESS);
        }

        public async Task<BaseResponseModel> Delete(Guid id)
        {
            var repo = _unitOfWork.Repository<ProgressLog, Guid>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            await repo.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.DELETE_SUCCESS);
        }

        public async Task<PaginatedList<ProgressLogsResponse>> getAllProgressLogs(PagingRequestModel model, ProgressLogsFillter fillter)
        {
            var baseSpeci = new BaseSpecification<ProgressLog>(pl =>
                (string.IsNullOrEmpty(fillter.QuitPlanName) || pl.QuitPlan.Reason.Contains(fillter.QuitPlanName)) &&
                (fillter.LogDate == default || pl.LogDate.Date == fillter.LogDate.Date) &&
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
    }
}
