using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Response
{
    public class MemberShipPackageResponse
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int DurationMonths { get; set; } // NEW: Thời hạn của gói (ví dụ: 0 cho Free, 1 cho 1 tháng, 12 cho 1 năm)
        public string? Features { get; set; } 
    }
}
