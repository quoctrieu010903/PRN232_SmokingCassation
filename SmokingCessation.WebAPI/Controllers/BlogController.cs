using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _service;

        public BlogController(IBlogService service)
        {
            _service = service;
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BaseResponseModel>> Create([FromBody] BlogRequest request)
        {
            var result = await _service.Create(request);
            return Ok(new BaseResponseModel(
                 statusCode: StatusCodes.Status200OK,
                 code: ResponseCodeConstants.SUCCESS,
                 data: MessageConstants.CREATE_SUCCESS));
        }
        [HttpGet]
        public async Task<ActionResult<PaginatedList<QuitPlansRequest>>> GetAll([FromQuery] PagingRequestModel paging)
        {
            var result = await _service.GetAll(paging);
            return Ok(new BaseResponseModel(
                     StatusCodes.Status200OK,
                     ResponseCodeConstants.SUCCESS,
                     result));
        }
        [HttpDelete("{id}")]
        [Authorize]

        public async Task<ActionResult<BaseResponseModel>> Delete(Guid id)
        {
            var result = await _service.Delete(id);
            return Ok(new BaseResponseModel<string>(
                 StatusCodes.Status200OK,
                 ResponseCodeConstants.SUCCESS,
                MessageConstants.DELETE_SUCCESS)); ;
        }
        [HttpPatch("{id}")]
        [Authorize]

        public async Task<ActionResult<BaseResponseModel>> Update(Guid id, [FromBody] BlogRequest request)
        {
            var result = await _service.Update(id,request);
            return Ok(new BaseResponseModel<string>(
                 StatusCodes.Status200OK,
                 ResponseCodeConstants.SUCCESS,
                 MessageConstants.UPDATE_SUCCESS)); ;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogResponse>> GetById(Guid id)
        {
            var result = await _service.GetBlogsDetails(id);
            return Ok(new BaseResponseModel(
                     StatusCodes.Status200OK,
                     ResponseCodeConstants.SUCCESS,
                     result));
        }
    }
}
