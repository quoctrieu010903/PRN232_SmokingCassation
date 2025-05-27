
using Microsoft.AspNetCore.Mvc;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.Service.Interface;

namespace SmokingCessation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IAuthenticationService _service;

        public IdentityController(IAuthenticationService service)
        {
            _service = service;
        }

        [HttpPost("userRole")]
         public async Task<IActionResult> AsignUserRole(AssignUserRoles request)
        {
            await _service.AssignUserRole(request);
            return Ok();
        }
        
    }
}
