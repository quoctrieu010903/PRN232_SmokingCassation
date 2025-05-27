using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Application.Service.Interface
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
        string GenerateRefeshToken();
    }
}
