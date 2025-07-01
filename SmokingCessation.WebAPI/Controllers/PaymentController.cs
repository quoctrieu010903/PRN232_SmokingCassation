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
            return Ok(new { paymentUrl = url });
        }

        [HttpGet("vnpay-return")]
            public async Task<IActionResult> VNPayReturn()
            {
                try
                {
              
                var result = await _paymentService.ProcessVnPayCallbackAsync(Request.Query);

                    // Auto-register user package here if success
                    if (result.isSuccess)
                        return Redirect("/payment-success"); // frontend route

                    return Redirect("/payment-failed");
                }
                catch
                {
                    return Redirect("/payment-error");
                }
            }
    }
}
