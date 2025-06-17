using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SmokingCessation.WebAPI.Controllers
{
    [Route("api/test")]
    [ApiController]
    [Authorize]

    public class TestController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping() => Ok("pong");
    }
}
