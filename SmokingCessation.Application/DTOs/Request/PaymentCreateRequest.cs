using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Request
{
    public class PaymentCreateRequest
    {
        public string MoneyUnit { get; set; }
        //TxnRef
        public Guid PackageId { get; set; }
        public string PaymentContent { get; set; } = "";
        public float TotalAmount { get; set; } = 0;
    }
}
