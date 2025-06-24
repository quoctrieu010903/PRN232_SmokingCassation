using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmokingCessation.Application.DTOs.Fillter;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Implementations;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.Response;
using SmokingCessation.Domain.Enums;
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
        public async Task<ActionResult<BaseResponseModel>> Create([FromForm] BlogRequest request)
        {
            var result = await _service.Create(request);
            return Ok(new BaseResponseModel(
                 statusCode: StatusCodes.Status200OK,
                 code: ResponseCodeConstants.SUCCESS,
                 data: MessageConstants.BLOG_CREATE_PENDING_APPROVAL));
        }
        /// <summary>
        /// /// Lấy danh sách blog theo phân quyền và filter:
        /// - User thường: chỉ thấy blog đã duyệt (Published).
        /// - Admin: xem được tất cả, có thể lọc theo trạng thái (Status).
        /// - Có thể lọc/sắp xếp theo FilterType (All, Newest, Popular) và tìm kiếm theo tiêu đề, nội dung, tác giả. 
        /// </summary>
        /// <param name="paging"></param>
        /// <param name="filterType"> có 3 loại all , new</param>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PaginatedList<QuitPlansRequest>>> GetAll([FromQuery] PagingRequestModel paging,
                                                [FromQuery] BlogListFilterType filterType = BlogListFilterType.Newest,
                                                [FromQuery] string? search = null,
                                                [FromQuery] BlogStatus? status = null

            )
            {
            var filter = new BlogListFilter { FilterType = filterType, Search = search ,Status = status };
            var result = await _service.GetAll(paging, filter);

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

        public async Task<ActionResult<BaseResponseModel>> Update(Guid id, [FromForm] BlogRequest request)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status">approve </param>
        /// <returns></returns>
        
        [HttpPatch("{id}/approve")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> ApproveBlog(Guid id)
        {
            try
            {
                var result = await _service.ChangeStatus(id, BlogStatus.Published);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPatch("{id}/reject")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> RejectBlog(Guid id)
        {
            try
            {
                var result = await _service.ChangeStatus(id, BlogStatus.Rejected);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPut("{id}/view")]
        public async Task<IActionResult> IncreaseViewCount(Guid id)
        {
            var result = await _service.IncreaseViewCount(id);
            return Ok(new BaseResponseModel(
                    StatusCodes.Status200OK,
                    ResponseCodeConstants.SUCCESS,
                    result));
        }
          

    }
}
