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
    public interface IBlogService
    {
        Task<BaseResponseModel> Create(BlogRequest request);
        Task<BaseResponseModel> Update(Guid id, BlogRequest request);
        Task<BaseResponseModel> Delete(Guid id);
        Task<PaginatedList<BlogResponse>> GetAll(PagingRequestModel model);
        Task<BaseResponseModel<BlogResponse>> GetBlogsDetails(Guid id);

    }
}
