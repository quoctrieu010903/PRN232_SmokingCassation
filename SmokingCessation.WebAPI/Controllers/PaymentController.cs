using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.Service.Implementations;
using SmokingCessation.Application.Service.Interface;

namespace SmokingCessation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVNPayService _paymentService;

        public PaymentController(IVNPayService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-url")]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] PaymentCreateRequest request)
        {
            var url = await _paymentService.GeneratePaymentUrlAsync(request);
            return Ok(url);
        }

        [HttpGet("vnpay-ipn")]
        public async Task<IActionResult> CallVNPayIPN()
        {
            return Ok(await _paymentService.CallVNPayIPN(Request.Query));
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VNPayReturn()
        {
            return Ok(await _paymentService.CallVNPayReturnUrl(Request.Query));
        }
    }
}