using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Core.Response;

namespace SmokingCessation.Application.Service.Interface
{
    public interface IVNPayService
    {
        Task<string> GeneratePaymentUrlAsync(PaymentCreateRequest request);
        Task<BaseResponseModel<VNPayResponseDTO>> CallVNPayReturnUrl(IQueryCollection vnpayData);
        Task<BaseResponseModel<TransactionResponseDTO>> CallVNPayIPN(IQueryCollection vnpayData);


    }
}
