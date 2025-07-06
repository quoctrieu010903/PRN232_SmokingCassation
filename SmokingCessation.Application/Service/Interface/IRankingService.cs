using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Core.Response;

namespace SmokingCessation.Application.Service.Interface
{
    public interface IRankingService
    {
        Task<BaseResponseModel<IEnumerable<UserRankingDetailDto>>> GetUserRankingsWithDetailsAsync();

    }
}
