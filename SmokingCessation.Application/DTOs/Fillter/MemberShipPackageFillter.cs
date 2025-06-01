using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Domain.Enums;

namespace SmokingCessation.Application.DTOs.Fillter
{
    public class MemberShipPackageFillter
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public MembershipType? Type { get; set; }
    }
}
