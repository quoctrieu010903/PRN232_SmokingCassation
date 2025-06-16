using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Domain.Entities;
using VNPAY_CS_ASPX;

namespace SmokingCessation.Application.Service.Implementations
{
    public class VNPaymentService : IVNPayService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public VNPaymentService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<string> GenerateUrlPayment(PaymentCreateRequest request)
        {
            var payCommand = _configuration["VNPay:PayCommand"]!;
            var vnp_tmnCode = _configuration["VNPay:TmnCode"]!;
            string vnp_locale = _configuration["VNPay:Locale"]!;
            var vnp_booking_orderType = _configuration["VNPay:BookingPackageType"]!;
            // var vnp_returnURL = URLPrefix.HTTPS + _httpContextAccessor.HttpContext!.Request.Host + _configuration["VNPay:ReturnUrl"]!;
            var vnp_returnURL = "https://skincare-booking-system.vercel.app/account-history";
            var vnp_url = _configuration["VNPay:Url"]!;
            var vnp_hashSet = _configuration["VNPay:HashSecret"]!;

            if (string.IsNullOrEmpty(vnp_tmnCode) || string.IsNullOrEmpty(vnp_hashSet))
            {
            }

            var ipAddress = Utils.GetIpAddress(_httpContextAccessor)!;
            //Build URL for VNPAY
            var vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", payCommand);
            vnpay.AddRequestData("vnp_TmnCode", vnp_tmnCode);
            vnpay.AddRequestData("vnp_Locale", vnp_locale);
            vnpay.AddRequestData("vnp_CurrCode", request.MoneyUnit);
            vnpay.AddRequestData("vnp_TxnRef", "payment-code_" + request.PackageId);
            vnpay.AddRequestData("vnp_OrderInfo", request.PaymentContent);
            vnpay.AddRequestData("vnp_OrderType", vnp_booking_orderType);
            vnpay.AddRequestData("vnp_Amount", (request.TotalAmount * 100).ToString());
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_returnURL);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(_httpContextAccessor)!);
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));

            string paymentUrl = vnpay.CreateRequestUrl(vnp_url, vnp_hashSet);
            return paymentUrl;
        }
    }
}
