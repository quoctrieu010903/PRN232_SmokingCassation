using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using System.Threading.Tasks;

namespace SmokingCessation.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _service;
        
        public AdminDashboardController(IAdminDashboardService service)
        {
            _service = service;
        }

        [HttpGet("summary")]
        [Authorize(Roles =UserRoles.Admin)]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _service.GetSummaryAsync();
            return Ok(result);
        }


        
    }
} 