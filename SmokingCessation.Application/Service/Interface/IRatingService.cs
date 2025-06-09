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
    public interface IRatingService
    {
        Task<BaseResponseModel> Create(RatingRequest request);
        Task<BaseResponseModel> Update(Guid id, RatingRequest request);
        Task<BaseResponseModel> Delete(Guid id);
        Task<PaginatedList<RatingResponse>> GetByBlogId(Guid blogId, PagingRequestModel paging);
        Task<PaginatedList<RatingResponse>> GetByUserId(Guid userId, PagingRequestModel paging);
        Task<BaseResponseModel<RatingResponse>> GetById(Guid id);

    }
}
