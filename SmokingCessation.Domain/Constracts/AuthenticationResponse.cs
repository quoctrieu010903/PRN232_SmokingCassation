using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Response
{
    public class AuthenticationResponse
    {
        public class UserResponse
        {
            public Guid Id { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string AccessToken { get; set; }


        }
        public class CurrentUserResponse
        {

            public Guid Id { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public string? RoleName { get; set; }
           public string? AccessToken { get; set; }

        }
      
        public class RevokeRefreshTokenResponse
        {
            public string Message { get; set; }
        }



    }
}
