using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.Response;

namespace SmokingCessation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CoachAdviceLogsController : ControllerBase
    {
        private readonly ICoachAdviceLogService _service;

        public CoachAdviceLogsController(ICoachAdviceLogService service)
        {
            _service = service;
        }

        /// <summary>
        /// Sinh và lưu lời khuyên mới cho user hiện tại (dựa vào quitplan hiện tại).
        /// </summary>
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateAdvice()
        {
            var response = await _service.GenerateAndSaveDailyAdviceAsync();
            return Ok(response);
        }

        /// <summary>
        /// Lấy tất cả lời khuyên của user hiện tại.
        /// </summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyAdvices()
        {
          

            var result = await _service.GetAdviceHistoryByUserAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy tất cả lời khuyên của một kế hoạch bỏ thuốc.
        /// </summary>
        [HttpGet("by-quitplan/{quitPlanId}")]
        public async Task<IActionResult> GetByQuitPlan(Guid quitPlanId)
        {
            var result = await _service.GetAllAdvicesByQuitPlanAsync(quitPlanId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết một lời khuyên.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetAdviceByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// (Admin) Lấy tất cả lời khuyên trong hệ thống.
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAdvicesAsync();
            return Ok(result);
        }

        /// <summary>
        /// Xóa một lời khuyên.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAdviceAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật nội dung lời khuyên.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] string newAdviceText)
        {
            var result = await _service.UpdateAdviceAsync(id, newAdviceText);
            return Ok(result);
        }
    }
}
