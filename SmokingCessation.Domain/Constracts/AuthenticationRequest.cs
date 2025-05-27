using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Core.Constants;

namespace SmokingCessation.Application.DTOs.Request
{
    public  class AuthenticationRequest
    {
        public class UserRegisterRequest
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string RoleName { get; set; } = UserRoles.Member;
           

        }
        public class UserLoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class UpdateUserRequest
        {
            public string FullName { get; set; }
          

            public string Email { get; set; }
            public string Password { get; set; }
           
        }
        public class RefreshTokenRequest
        {
            public string RefreshToken { get; set; }
        }


    }
}
