using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending = 10,
        Completed = 20,
        Failed = 30,
        Refunded = 40
    }
}
