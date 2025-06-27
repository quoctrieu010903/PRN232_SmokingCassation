using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SmokingCessation.Application.DTOs.Fillter;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.Response;
using SmokingCessation.Domain.Specifications;

namespace SmokingCessation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressLogsController : ControllerBase
    {
        private readonly IProgressLogsService _service;

        public ProgressLogsController(IProgressLogsService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BaseResponseModel>> Create([FromBody] ProgressLogsRequest request)
        {
            var result = await _service.Create(request);
            return Ok(result);
        }
        [HttpGet]
        public async Task<ActionResult<PaginatedList<QuitPlansRequest>>> GetAll([FromQuery] PagingRequestModel paging, [FromQuery] ProgressLogsFillter filter)
        {
            var result = await _service.getAllProgressLogs(paging, filter);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        [Authorize]

        public async Task<ActionResult<BaseResponseModel>> Delete(Guid id)
        {
            var result = await _service.Delete(id);
            return Ok(result);
        }
        [HttpPatch("{id}")]
        [Authorize]

        public async Task<ActionResult<BaseResponseModel>> Update(Guid id, [FromBody] ProgressLogsRequest request)
        {
            var result = await _service.Update(request, id);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberShipPackageResponse>> GetById(Guid id)
        {
            var result = await _service.getProgressLogsyId(id);
            return Ok(result);
        }
        /// <summary>
        /// Tạo ProgressLog dựa trên lời khuyên mới nhất của user hiện tại tu coachadvices.
        /// </summary>
        [HttpPost("from-advice")]
        public async Task<IActionResult> CreateFromAdvice()
        {
            var response = await _service.CreateProgressLogFromAdviceAsync();
            return Ok(response);
        }
    }
}
