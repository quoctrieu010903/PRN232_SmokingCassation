using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SmokingCessation.WebAPI.Controllers
{
    [Route("api/test")]
    [ApiController]

    public class TestController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping() => Ok("pong");
    }
}
