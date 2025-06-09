using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
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
        private readonly IUserContext _userContext;

        public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper, IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userContext = userContext;
        }

        //public async Task<BaseResponseModel> Approve(Guid id, bool isApproved)
        //{
        //    var repo = _unitOfWork.Repository<Feedback, Guid>();
        //    var feedback = await repo.GetByIdAsync(id);
        //    if (feedback == null)
        //        return new BaseResponseModel(404, "NOT_FOUND", "Feedback not found");

           
        //    await repo.UpdateAsync(feedback);
        //    await _unitOfWork.SaveChangesAsync();
        //    return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, isApproved ? "Feedback approved" : "Feedback rejected");

        //}

        public async Task<BaseResponseModel> Create(FeedbackRequest request)
        {

            var userId = _userContext.GetUserId();
            var feedback = new Feedback
            {
                BlogId = request.BlogId,
                UserId = Guid.Parse(userId),
                Comment = request.Comment,
          
                CreatedBy = userId,
                CreatedTime = DateTimeOffset.UtcNow
            };
            await _unitOfWork.Repository<Feedback, Guid>().AddAsync(feedback);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(200, "SUCCESS", "Feedback created, pending approval");

        }

        public async Task<BaseResponseModel> Delete(Guid id)
        {

            var userId = _userContext.GetUserId();
            var repo = _unitOfWork.Repository<Feedback, Guid>();
            var feedback = await repo.GetByIdAsync(id);
            if (feedback == null)
                return new BaseResponseModel(404, "NOT_FOUND", "Feedback not found");

            feedback.DeletedBy = userId;
            feedback.DeletedTime = CoreHelper.SystemTimeNow;
            feedback.LastUpdatedBy = userId;
            feedback.LastUpdatedTime = CoreHelper.SystemTimeNow;
            
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
                return new BaseResponseModel<FeedbackResponse>(404, "NOT_FOUND", "Feedback not found");

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
            var repo = _unitOfWork.Repository<Feedback, Guid>();
            var feedback = await repo.GetByIdAsync(id);
            if (feedback == null)
                return new BaseResponseModel(404, "NOT_FOUND", "Feedback not found");

            feedback.Comment = request.Comment;
            await repo.UpdateAsync(feedback);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(200, "SUCCESS", "Feedback updated");

        }
    }
}
