using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SmokingCessation.Application.Service.Implementations
{
    public record CurrentUser(string UserId, string email, IEnumerable<string> Roles)
    {

        public bool isInRole(string role) => Roles.Contains(role);

    }
}

