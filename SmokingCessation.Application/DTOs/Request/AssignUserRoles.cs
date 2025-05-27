using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Request
{
    public class AssignUserRoles
    {
        public String Email { get; set; }
        public string RoleName { get; set; }

    }
}
