using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Core.Base;
using SmokingCessation.Domain.Enums;

namespace SmokingCessation.Domain.Entities
{
    public class MembershipPackage : BaseEntity
    {
        
        public string Name { get; set; }
        public decimal Price { get; set; }
        public MembershipType Type { get; set; } 
        public string Description { get; set; }
        public int DurationMonths { get; set; } // NEW: Thời hạn của gói (ví dụ: 0 cho Free, 1 cho 1 tháng, 12 cho 1 năm)
        public string? Features { get; set; } // NEW: Liệt kê các tính năng của gói




        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<QuitPlan> QuitPlans { get; set; }
    }
}
