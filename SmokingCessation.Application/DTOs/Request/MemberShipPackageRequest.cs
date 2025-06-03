

using System.Reflection;
using SmokingCessation.Domain.Enums;

namespace SmokingCessation.Application.DTOs.Request
{
    public class MemberShipPackageRequest
    {
        public string Name { get; set; }
        public MembershipType Type { get; set; } 
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int DurationMonths { get; set; } 
        public string? Features { get; set; }

    }
}
