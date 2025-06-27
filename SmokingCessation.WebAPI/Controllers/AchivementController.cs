using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Response;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Core.Constants;

namespace SmokingCessation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AchivementController : ControllerBase
    {
        private readonly IAchivementServie _service;

        public AchivementController(IAchivementServie service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseModel<AchievementResponse>>> Create([FromForm] AchievementCreateRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Ok(new BaseResponseModel(
              statusCode: StatusCodes.Status200OK,
              code: ResponseCodeConstants.SUCCESS,
              data: MessageConstants.CREATE_SUCCESS));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponseModel<AchievementResponse?>>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(new BaseResponseModel(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               data: result));
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseModel<IEnumerable<AchievementResponse>>>> GetAll([FromQuery] PagingRequestModel paging)
        {
            var result = await _service.GetAllAsync(paging);
            return Ok(new BaseResponseModel(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: result));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponseModel<AchievementResponse?>>> Update([FromForm] AchievementUpdateRequest request, Guid id)
        {
            var result = await _service.UpdateAsync(request, id);
            return Ok(new BaseResponseModel(
             statusCode: StatusCodes.Status200OK,
             code: ResponseCodeConstants.SUCCESS,
             data: result));
        }
    }
}
