using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmokingCessation.Application.DTOs.Response.AuthenticationResponse;

namespace SmokingCessation.Application.Helpers
{
    public  interface IJwtHelper
    {
        void DecryptAccessToken(CurrentUserResponse currentLoginAccountDTO, string accessToken);
        string ComputeSha256Hash(string refreshToken);
    }
}
