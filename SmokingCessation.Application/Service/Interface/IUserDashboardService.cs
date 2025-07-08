using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Core.Response;

namespace SmokingCessation.Application.Service.Interface
{
    public interface IUserDashboardService
    {
        // Define methods for user dashboard service
        // For example, you might have methods to get user statistics, progress, etc.

        Task<BaseResponseModel<UserDashboardDto>> GetUserStatisticsAsync(Guid userId, DateTime? week = null, DateTime? month = null);
      


        // Add more methods as needed for the user dashboard functionality
    }
}
