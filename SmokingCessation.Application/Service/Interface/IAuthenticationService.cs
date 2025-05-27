using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Core.Response;

namespace SmokingCessation.Application.Service.Interface
{
    public interface IAuthenticationService
    {
        Task AssignUserRole(AssignUserRoles request);
    }
}
