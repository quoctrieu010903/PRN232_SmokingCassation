using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Request
{
    public class VnPaymentRequestModel
    {
        public string FullName { get; set; }
        public string Description { get; set; }
        public string Amount { get; set; }
    }
}
