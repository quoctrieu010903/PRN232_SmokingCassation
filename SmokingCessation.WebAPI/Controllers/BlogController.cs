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
            return Ok(result);
        }
        /// <summary>
        /// Lấy danh sách blog theo phân quyền và filter:
        /// - User thường: chỉ thấy blog đã duyệt (Published).
        /// - Admin: xem được tất cả, có thể lọc theo trạng thái (Status).
        /// - Có thể lọc/sắp xếp theo FilterType (All, Newest, Popular) và tìm kiếm theo tiêu đề, nội dung, tác giả. 
        /// </summary>
        /// <param name="paging"></param>
        /// <param name="filterType"> có 3 loại all : 0 ,  new : 1 ,  Popular : 2 </param>
        /// <param name="search"></param>
        /// <param name="status">  Pending_Approval : 0,   Rejected: 1,  Published : 2  -  Role Admin</param>

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

            return Ok(result);
        }

        /// <summary>
        /// Get blogs by author. If no authorId is provided, get blogs by current user.
        /// lấy tất cả blog bởi author , nếu không có authorID thì sẽ lấy các blog của user hiện tại ( phải logged in users 
        /// </summary>
        [HttpGet("author")]
        [Authorize] // Optional: use this if you want to restrict to logged-in users
        public async Task<IActionResult> GetBlogsByAuthor([FromQuery] Guid? authorId, [FromQuery] PagingRequestModel pagingModel)
        {
            var result = await _service.GetBlogByAuthor(authorId, pagingModel);
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

        public async Task<ActionResult<BaseResponseModel>> Update(Guid id, [FromForm] BlogRequest request)
        {
            var result = await _service.Update(id,request);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogResponse>> GetById(Guid id)
        {
            var result = await _service.GetBlogsDetails(id);
            return Ok(result);
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
            return Ok(result);
        }
          

    }
}
