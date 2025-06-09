using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Core.Response;
using SmokingCessation.Domain.Specifications;

namespace SmokingCessation.Application.Service.Interface
{
    public interface IFeedbackService
    {
        Task<BaseResponseModel> Create(FeedbackRequest request);
        Task<BaseResponseModel> Update(Guid id, FeedbackRequest request);
        Task<BaseResponseModel> Delete(Guid id);    
        Task<PaginatedList<FeedbackResponse>> GetByBlogId(Guid blogId, PagingRequestModel paging);
        Task<PaginatedList<FeedbackResponse>> GetByUserId(Guid userId, PagingRequestModel paging);
        Task<BaseResponseModel<FeedbackResponse>> GetById(Guid id);

    }
}
