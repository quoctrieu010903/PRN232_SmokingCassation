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
    public class MemberShipPackageController : ControllerBase
    {
        private readonly IMemberShipPackage _service;

        public MemberShipPackageController(IMemberShipPackage service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedList<MemberShipPackageResponse>>> GetAll([FromQuery] PagingRequestModel paging, [FromQuery] MemberShipPackageFillter filter)
        {
            var result = await _service.getAllPackage(paging, filter);
            return  Ok( result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberShipPackageResponse>> GetById(Guid id)
        {
           var result = await _service.getPackageById(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BaseResponseModel>> Create([FromBody] MemberShipPackageRequest request)
        {
            var result = await _service.Create(request);
            return Ok(result);
        }
        [HttpPatch("{id}")]
        [Authorize]

        public async Task<ActionResult<BaseResponseModel>> Update(Guid id, [FromBody] MemberShipPackageRequest request)
        {
            var result = await _service.Update(request, id);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]

        public async Task<ActionResult<BaseResponseModel>> Delete(Guid id)
        {
            var result = await _service.Delete(id);
            return Ok(result);
        }


    }
}
