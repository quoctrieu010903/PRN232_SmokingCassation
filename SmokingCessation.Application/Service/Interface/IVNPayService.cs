using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Application.DTOs.Request;

namespace SmokingCessation.Application.Service.Interface
{
    public interface IVNPayService
    {
        Task<string> GenerateUrlPayment(PaymentCreateRequest request);
    }
}
