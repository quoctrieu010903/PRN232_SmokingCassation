using System.Threading.Tasks;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Core.Response;

namespace SmokingCessation.Application.Service.Interface
{
    public interface IAdminDashboardService
    {
        Task<BaseResponseModel<AdminDashboardSummaryDto>> GetSummaryAsync();

    }
} 