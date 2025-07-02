using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Exceptions;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.CustomExceptionss;
using SmokingCessation.Core.Response;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Enums;
using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Domain.Specifications;
using VNPAY_CS_ASPX;

namespace SmokingCessation.Application.Service.Implementations
{
    public class VNPaymentService : IVNPayService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VNPaymentService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IUnitOfWork unitOfWork , IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel<TransactionResponseDTO>> CallVNPayIPN(IQueryCollection vnpayData)
        {

            var vnpHelper = new VnPayLibrary();
            var vnpayResponseDTO = GetAllVNPayResponseData(vnpayData, vnpHelper);
            var txnRef = vnpayResponseDTO.vnp_TxnRef;

            // Tìm Payment theo VnPayTxnRef
            var paymentRepo = _unitOfWork.Repository<Payment, Guid>();
            var baseSpecific = new BaseSpecification<Payment>(p => p.VnPayTxnRef == txnRef);
            var payment = await paymentRepo.GetWithSpecAsync(baseSpecific, true);

            // Parse txnRef để lấy UserId và PaymentId
            var parts = vnpayResponseDTO.vnp_TxnRef.Split('_');
            if (parts.Length < 3) throw new APIException(400, "Sai định dạng mã giao dịch");

            var userId = Guid.Parse(parts[1]);
            var paymentId = Guid.Parse(parts[2]);

            // Kiểm tra phản hồi từ VNPay
            if (!vnpayResponseDTO.isSuccess)
            {
                payment.Status = PaymentStatus.Failed;
                throw new APIException((int)HttpStatusCode.BadRequest, "Thanh toán không thành công từ VNPay");
            }

            if (payment == null)
                throw new APIException(404, "Không tìm thấy giao dịch");

            if (payment.Status == PaymentStatus.Success)
                throw new ErrorException(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, "Giao dịch đã được xử lý");
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Cập nhật trạng thái
                payment.Status = PaymentStatus.Success;
                payment.CreatedTime = DateTime.UtcNow;

                // Tạo gói
                var package = await _unitOfWork.Repository<MembershipPackage, Guid>().GetByIdAsync(payment.PackageId);
                if (package == null)
                    throw new APIException(404, "Gói không tồn tại");

                var userPackage = new UserPackage
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    PackageId = payment.PackageId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(package.DurationMonths),
                    IsActive = true,
                    CreatedBy = userId.ToString(),
                    CreatedTime = DateTime.UtcNow,
                };

                await _unitOfWork.Repository<UserPackage, Guid>().AddAsync(userPackage);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                var result = _mapper.Map<TransactionResponseDTO>(payment);
                return new BaseResponseModel<TransactionResponseDTO>(200, "SUCCESS", result);
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

        }

        public async Task<BaseResponseModel<VNPayResponseDTO>> CallVNPayReturnUrl(IQueryCollection vnpayData)
        {
            var vNPayHelper = new VnPayLibrary();

            //get all vnpay response data
            VNPayResponseDTO vnpayResponseDTO = GetAllVNPayResponseData(vnpayData, vNPayHelper);
            return new BaseResponseModel<VNPayResponseDTO>(StatusCodes.Status200OK, MessageConstants.CREATE_SUCCESS, vnpayResponseDTO); 
        }

       

        public async Task<string> GeneratePaymentUrlAsync(PaymentCreateRequest request)
        {
            // Step 1: Tạo bản ghi Payment trạng thái Pending
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                PackageId = request.BookingId, 
                Amount = (decimal)request.TotalAmount,
                Status = PaymentStatus.Pending,
                VnPayTxnRef = $"PAYMENT_{request.UserId}_{request.BookingId}", 
                CreatedTime = DateTime.UtcNow,
                CreatedBy = request.UserId.ToString()
            };

            await _unitOfWork.Repository<Payment, Guid>().AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();
            // Step 2: Tạo URL thanh toán VNPay

            var payCommand = _configuration["VNPay:PayCommand"]!;
            var vnp_tmnCode = _configuration["VNPay:TmnCode"]!;
            string vnp_locale = _configuration["VNPay:Locale"]!;
            var vnp_booking_orderType = _configuration["VNPay:BookingPackageType"]!;
             var vnp_returnURL = URLPrefix.HTTP + _httpContextAccessor.HttpContext!.Request.Host + _configuration["VNPay:ReturnUrl"]!;
            
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
            vnpay.AddRequestData("vnp_TxnRef", $"PAYMENT_{request.UserId}_{request.BookingId}");
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
        private VNPayResponseDTO GetAllVNPayResponseData(IQueryCollection vnpayData, VnPayLibrary vNPayHelper)
        {
            foreach (var s in vnpayData)
            {
                //get all querystring data
                if (!string.IsNullOrEmpty(s.Value) && s.Key.StartsWith("vnp_"))
                {
                    vNPayHelper.AddResponseData(s.Key, s.Value);
                }
            }
            var vnp_ResponseCode = vNPayHelper.GetResponseData("vnp_ResponseCode");
            var vnp_TransactionStatus = vNPayHelper.GetResponseData("vnp_TransactionStatus");
            var vnp_TmnCode = vNPayHelper.GetResponseData("vnp_TmnCode");
            var vnp_TxnRef = vNPayHelper.GetResponseData("vnp_TxnRef");
            float vnp_Amount = float.Parse(vNPayHelper.GetResponseData("vnp_Amount")) / 100;
            var vnp_OrderInfo = vNPayHelper.GetResponseData("vnp_OrderInfo");
            var vnp_BankCode = vNPayHelper.GetResponseData("vnp_BankCode");
            var vnp_BankTranNo = vNPayHelper.GetResponseData("vnp_BankTranNo");
            var vnp_CardType = vNPayHelper.GetResponseData("vnp_CardType");
            var vnp_PayDate = vNPayHelper.GetResponseData("vnp_PayDate");
            var vnp_TransactionNo = vNPayHelper.GetResponseData("vnp_TransactionNo");
            var vnp_SecureHash = vNPayHelper.GetResponseData("vnp_SecureHash");
            var vnpay_hash_secret = _configuration["VNPay:HashSecret"];
            bool checkSignature = vNPayHelper.ValidateSignature(vnp_SecureHash, vnpay_hash_secret);
            if (!checkSignature)
            {

                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);
            }
            bool isSuccess = true;
            if (vnp_ResponseCode != "00" || vnp_TransactionStatus != "00")
            {
                isSuccess = false;
            }
            return new VNPayResponseDTO
            {
                vnp_TmnCode = vnp_TmnCode,
                vnp_TxnRef = vnp_TxnRef,
                vnp_Amount =Convert.ToSingle(vnp_Amount),
                vnp_OrderInfo = vnp_OrderInfo,
                vnp_BankCode = vnp_BankCode,
                vnp_BankTranNo = vnp_BankTranNo,
                vnp_CardType = vnp_CardType,
                vnp_PayDate = vnp_PayDate,
                vnp_TransactionNo = vnp_TransactionNo,
                isSuccess = isSuccess
            };
        }

        public async Task<VnPaymentResponseModel> ProcessVnPayCallbackAsync(IQueryCollection vnpayData)
        {
            var vNPayHelper = new VnPayLibrary();
            var response = GetAllVNPayResponseData(vnpayData, vNPayHelper);

            var userId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst("id")!.Value);
            var txnRef = response.vnp_TxnRef;
            var packageIdStr = txnRef.Replace("payment-code_", "");
            var packageId = Guid.Parse(packageIdStr);

            var paymentRepo = _unitOfWork.Repository<Payment, Guid>();
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PackageId = packageId,
                Amount = (decimal) response.vnp_Amount,
                Status = response.isSuccess ? PaymentStatus.Success : PaymentStatus.Failed,
                CreatedTime = DateTime.UtcNow,
               
            };

            await paymentRepo.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return new VnPaymentResponseModel
            {
                isSuccess = response.isSuccess,
                Message = response.isSuccess ? "Payment success." : "Payment failed.",
                Amount = (decimal) response.vnp_Amount,
                PackageId = packageId
            };
        }


    }
}
