using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Response
{
    public class VnPaymentResponseModel
    {
        public Guid PaymentId { get; set; }
        public bool isSuccess { get; set; }
        public decimal Amount { get; set; }
        public Guid PackageId { get; set; }
        public string Message { get; set; } = "";

    }
}
