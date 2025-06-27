using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmokingCessation.Application.DTOs.Fillter;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.Response;
using SmokingCessation.Domain.Enums;
using SmokingCessation.Domain.Specifications;

namespace SmokingCessation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuitPlanController : ControllerBase
    {
        private readonly IQuitPlanService _service;

        public QuitPlanController(IQuitPlanService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BaseResponseModel>> Create([FromBody] QuitPlansRequest request)
        {
            var result = await _service.Create(request);
            return Ok(result);
        }
        [HttpGet]
        public async Task<ActionResult<PaginatedList<QuitPlansRequest>>> GetAll([FromQuery] PagingRequestModel paging, [FromQuery] QuitPlanFillter filter, bool isCurrentUser)
        {
            var result = await _service.getAllQuitPlan(paging, filter, isCurrentUser);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        [Authorize]

        public async Task<ActionResult<BaseResponseModel>> Delete(Guid id)
        {
            var result = await _service.Delete(id);
            return Ok(result); 
        }
        [HttpPatch("{id}/Activate")]
        [Authorize]
        public async Task<ActionResult<BaseResponseModel>> SetActive(Guid id)
        {
            var result = await _service.UpdateStatus(id, QuitPlanStatus.Active);
            return Ok(result);
        }

        [HttpPatch("{id}/deactivate")]
        [Authorize]
        public async Task<ActionResult<BaseResponseModel>> SetInactive(Guid id)
        {
            var result = await _service.UpdateStatus(id,QuitPlanStatus.Inactive );
            return Ok(result);
        }

        [HttpPatch("{id}/complete")]
        [Authorize]
        public async Task<ActionResult<BaseResponseModel>> SetCompleted(Guid id)
        {
            var result = await _service.UpdateStatus(id, QuitPlanStatus.Completed);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberShipPackageResponse>> GetById(Guid id)
        {
            var result = await _service.getQuitPlanById(id);
            return Ok(result);
        }

    }
}
