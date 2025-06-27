using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Core.Response;

namespace SmokingCessation.Application.Service.Interface
{
    public interface IAchivementServie 
    {
        Task<BaseResponseModel<AchievementResponse>> CreateAsync(AchievementCreateRequest request);
        Task<BaseResponseModel<AchievementResponse?>> GetByIdAsync(Guid id);
        Task<BaseResponseModel<IEnumerable<AchievementResponse>>> GetAllAsync( PagingRequestModel paging);
        Task<BaseResponseModel<AchievementResponse?>> UpdateAsync(AchievementUpdateRequest request, Guid id);


    }
}
