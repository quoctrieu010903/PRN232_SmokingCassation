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
    public interface IQuitPlanService
    {
        Task<PaginatedList<QuitPlanResponse>> getAllQuitPlan(PagingRequestModel model, QuitPlanFillter fillter, bool isCurrentUser);
        Task<BaseResponseModel<QuitPlanResponse>> getQuitPlanById(Guid id);
        Task<BaseResponseModel> Create(QuitPlansRequest request);
        Task<BaseResponseModel> Update(QuitPlansRequest request, Guid id);
        Task<BaseResponseModel> Delete(Guid id);
    }
}
