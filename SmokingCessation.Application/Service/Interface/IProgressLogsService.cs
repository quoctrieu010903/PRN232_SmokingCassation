using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Application.DTOs.Fillter;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Core.Response;
using SmokingCessation.Domain.Specifications;

namespace SmokingCessation.Application.Service.Interface
{
    public interface IProgressLogsService
    {
        Task<PaginatedList<ProgressLogsResponse>> getAllProgressLogs(PagingRequestModel model, ProgressLogsFillter fillter);
        Task<BaseResponseModel<ProgressLogsResponse>> getProgressLogsyId(Guid id);
        Task<BaseResponseModel> Create(ProgressLogsRequest request);
        Task<BaseResponseModel> Update(ProgressLogsRequest request, Guid id);
        Task<BaseResponseModel> Delete(Guid id);
    }
}
