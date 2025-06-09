using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _service;

        public FeedbackController(IFeedbackService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] FeedbackRequest request)
        {
            var result = await _service.Create(request);
            return Ok(new BaseResponseModel(
                      StatusCodes.Status200OK,
                      ResponseCodeConstants.SUCCESS,
                      result));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] FeedbackRequest request)
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

        //[HttpPut("{id}/approve")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> Approve(Guid id, [FromQuery] bool isApproved)
        //{
        //    var result = await _service.Approve(id, isApproved);
        //    return Ok(new BaseResponseModel(
        //                StatusCodes.Status200OK,
        //                ResponseCodeConstants.SUCCESS,
        //                result));
        //}

        [HttpGet("by-blog/{blogId}")]
        public async Task<IActionResult> GetByBlogId(Guid blogId, [FromQuery] PagingRequestModel paging)
        {
            var data = await _service.GetByBlogId(blogId, paging);
            var response = BaseResponseModel<PaginatedList<FeedbackResponse>>.OkResponseModel(data);
            return Ok(new BaseResponseModel(
                       StatusCodes.Status200OK,
                       ResponseCodeConstants.SUCCESS,
                       response));
        }

        [HttpGet("by-user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetByUserId(Guid userId, [FromQuery] PagingRequestModel paging)
        {
            var data = await _service.GetByUserId(userId, paging);
            var response = BaseResponseModel<PaginatedList<FeedbackResponse>>.OkResponseModel(data);
            return Ok(new BaseResponseModel(
                        StatusCodes.Status200OK,
                        ResponseCodeConstants.SUCCESS,
                        response));
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

