using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.Response;

namespace SmokingCessation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPackage : ControllerBase
    {
        private readonly IUserPackageService _service;

        public UserPackage(IUserPackageService service)
        {
            _service = service;
        }

        /// <summary>
        /// Đăng ký gói thành viên mới (sau khi đã thanh toán thành công)
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserPackageRequest request)
        {
            var result = await _service.RegisterPackage(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy gói thành viên hiện tại còn hạn
        /// </summary>
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent()
        {
            var result = await _service.GetCurrentPackage();
            return Ok(result);
        }
        /// <summary>
        /// Hủy gói thành viên hiện tại
        /// </summary>
        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel()
        {
            var result = await _service.CancelCurrentPackage();
            return Ok(result);
        }

        /// <summary>
        /// Lấy lịch sử các gói đã đăng ký
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var result = await _service.GetPackageHistory();
            return Ok(result);
        }
            /// <summary>
            /// Lấy chi tiết một gói đã đăng ký theo Id
            /// </summary>
            [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetPackageById(id);
            return Ok(result);
        }

    }
}
