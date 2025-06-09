using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
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
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _service;

        public RatingController(IRatingService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] RatingRequest request)
        {
            var result = await _service.Create(request);
          
            return Ok(new BaseResponseModel(
                     StatusCodes.Status200OK,
                     ResponseCodeConstants.SUCCESS,
                     result));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] RatingRequest request)
        {
            var result = await _service.Update(id, request);
            return Ok(new BaseResponseModel(
                     StatusCodes.Status200OK,
                     ResponseCodeConstants.SUCCESS,
                     result));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.Delete(id);
            return Ok(new BaseResponseModel(
                      StatusCodes.Status200OK,
                      ResponseCodeConstants.SUCCESS,
                      result));
        }

        [HttpGet("by-blog/{blogId}")]
        public async Task<ActionResult<PaginatedList<RatingResponse>>> GetByBlogId(Guid blogId, [FromQuery] PagingRequestModel paging)
        {
            var result = await _service.GetByBlogId(blogId, paging);
            return Ok(new BaseResponseModel(
                    StatusCodes.Status200OK,
                    ResponseCodeConstants.SUCCESS,
                    result));
        }

        [HttpGet("by-user/{userId}")]
        [Authorize]
        public async Task<ActionResult<PaginatedList<RatingResponse>>> GetByUserId(Guid userId, [FromQuery] PagingRequestModel paging)
        {
            var result = await _service.GetByUserId(userId, paging);
            return Ok(new BaseResponseModel(
                      StatusCodes.Status200OK,
                      ResponseCodeConstants.SUCCESS,
                      result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetById(id);
            return Ok(new BaseResponseModel(
                       StatusCodes.Status200OK,
                       ResponseCodeConstants.SUCCESS,
                       result));
        }
    }
}

