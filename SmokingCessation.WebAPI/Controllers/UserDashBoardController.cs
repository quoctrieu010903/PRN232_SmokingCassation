using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Response;

namespace SmokingCessation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDashBoardController : ControllerBase
    {
        private readonly IUserDashboardService _userDashboardService;
        public UserDashBoardController(IUserDashboardService userDashboardService)
        {
            _userDashboardService = userDashboardService;
        }
        [HttpGet("statistics/{userId}")]
        public async Task<ActionResult<BaseResponseModel<UserDashboardDto>>> GetUserStatistics(Guid userId, [FromQuery]DateTime? week = null, [FromQuery]DateTime? month = null)
        {
            var response = await _userDashboardService.GetUserStatisticsAsync(userId, week, month);
            return (response);
        }
    }
}
