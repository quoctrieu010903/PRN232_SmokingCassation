using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.CustomExceptionss;
using SmokingCessation.Core.Response;
using SmokingCessation.Core.Utils;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Domain.Specifications;

namespace SmokingCessation.Application.Service.Implementations
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        //public async Task<BaseResponseModel> Approve(Guid id, bool isApproved)
        //{
        //    var repo = _unitOfWork.Repository<Feedback, Guid>();
        //    var feedback = await repo.GetByIdAsync(id);
        //    if (feedback == null)
        //       throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

           
        //    await repo.UpdateAsync(feedback);
        //    await _unitOfWork.SaveChangesAsync();
        //    return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, isApproved ? "Feedback approved" : "Feedback rejected");

        //}

        public async Task<BaseResponseModel> Create(FeedbackRequest request)
        {

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var feedback = new Feedback
            {
                BlogId = request.BlogId,
                UserId = Guid.Parse(userId),
                Comment = request.Comment,
          
                CreatedBy = userId,
                CreatedTime =  DateTime.UtcNow,
                LastUpdatedBy = userId,
                LastUpdatedTime =  DateTime.UtcNow,
            };
            await _unitOfWork.Repository<Feedback, Guid>().AddAsync(feedback);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(200, "SUCCESS", "Feedback created, pending approval");

        }

        public async Task<BaseResponseModel> Delete(Guid id)
        {

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var repo = _unitOfWork.Repository<Feedback, Guid>();
            var feedback = await repo.GetByIdAsync(id);
            if (feedback == null)
               throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            feedback.DeletedBy = userId;
            feedback.DeletedTime =  DateTime.UtcNow;
            feedback.LastUpdatedBy = userId;
            feedback.LastUpdatedTime =  DateTime.UtcNow;
            
            await repo.UpdateAsync(feedback);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(200, "SUCCESS", "Feedback deleted");

        }

        public async Task<PaginatedList<FeedbackResponse>> GetByBlogId(Guid blogId, PagingRequestModel paging)
        {
            var baseSpeci = new Domain.Specifications.BaseSpecification<Feedback>(f => f.BlogId == blogId);
            var feedbacks = await _unitOfWork.Repository<Feedback, Feedback>().GetAllWithSpecAsync(baseSpeci);
            var result = _mapper.Map<List<FeedbackResponse>>(feedbacks);
            return PaginatedList<FeedbackResponse>.Create(result, paging.PageNumber, paging.PageSize);

        }

        public async Task<BaseResponseModel<FeedbackResponse>> GetById(Guid id)
        {
            var repo = _unitOfWork.Repository<Feedback, Guid>();
            var feedback = await repo.GetByIdAsync(id);
            if (feedback == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            var result = _mapper.Map<FeedbackResponse>(feedback);
            return new BaseResponseModel<FeedbackResponse>(200, "SUCCESS", result);

        }

        public async Task<PaginatedList<FeedbackResponse>> GetByUserId(Guid userId, PagingRequestModel paging)
        {
            var baseSpeci = new BaseSpecification<Feedback>(f => f.UserId == userId);
            var feedbacks = await _unitOfWork.Repository<Feedback, Feedback>().GetAllWithSpecAsync(baseSpeci);
            var result = _mapper.Map<List<FeedbackResponse>>(feedbacks);
            return PaginatedList<FeedbackResponse>.Create(result, paging.PageNumber, paging.PageSize);
        }

        public async Task<BaseResponseModel> Update(Guid id, FeedbackRequest request)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var repo = _unitOfWork.Repository<Feedback, Guid>();
            var feedback = await repo.GetByIdAsync(id);
            if (feedback == null)
               throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);
            feedback.DeletedBy = userId;
            feedback.DeletedTime = DateTime.UtcNow;    
            feedback.LastUpdatedBy = userId;
            feedback.LastUpdatedTime = DateTime.UtcNow;

            feedback.Comment = request.Comment;
            await repo.UpdateAsync(feedback);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(200, "SUCCESS", "Feedback updated");

        }
    }
}
