using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmokingCessation.Application.Exceptions;
using SmokingCessation.Core.Constants;
using SmokingCessation.Domain.Entities;
using static SmokingCessation.Application.DTOs.Response.AuthenticationResponse;

namespace SmokingCessation.Application.Helpers
{
    public class JWTHelper
    {
        private readonly IConfiguration _configuration;

        public JWTHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ComputeSha256Hash(string refreshToken)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));

                // Convert bytes to hex string  
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2")); // "x2" formats as hex  

                return builder.ToString();
            }
        }
    }

    public static class JWTHelperExtensions
    {
        //public static void DecryptAccessToken(this CurrentUserResponse currentLoginAccountDTO, string accessToken, IConfiguration configuration)
        //{
        //    string accessTokenPrefix = "Bearer ";
        //    if (accessToken.Contains(accessTokenPrefix))
        //    {
        //        accessToken = accessToken.Substring(accessTokenPrefix.Length);
        //    }
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var tokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuer = true,
        //        ValidateAudience = true,
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:Key"])),
        //        ValidIssuer = configuration["JwtConfig:Issuer"],
        //        ValidAudience = configuration["JwtConfig:Audience"]
        //    };
        //    var validToken = tokenHandler.CanReadToken(accessToken);
        //    if (!validToken)
        //    {
        //        throw new APIException((int)HttpStatusCode.BadRequest, "Invalid access token");
        //    }
        //    var jwtToken = tokenHandler.ReadJwtToken(accessToken);
        //    if (jwtToken.ValidTo.CompareTo(DateTime.UtcNow) < 0)
        //    {
        //        throw new APIException((int)HttpStatusCode.Unauthorized, ResponseCodeConstants.JWT_TOKEN_EXPIRED);
        //    }
        //    ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(accessToken.Trim(), tokenValidationParameters, out _);
        //    if (claimsPrincipal == null)
        //    {
        //        throw new APIException((int)HttpStatusCode.BadRequest, "Failed to decrypt/validate access token");
        //    }
        //    currentLoginAccountDTO.Id = Guid.Parse(claimsPrincipal.FindFirstValue("UserId"));
        //    currentLoginAccountDTO.Email = claimsPrincipal.FindFirstValue("Email");
        //    currentLoginAccountDTO.FullName = claimsPrincipal.FindFirstValue("FullName");
        //    currentLoginAccountDTO.RoleName = claimsPrincipal.FindFirstValue("Role");
        //}

        public static ClaimsPrincipal DecryptAccessToken(this CurrentUserResponse currentLoginAccountDTO, string accessToken, IConfiguration configuration)
        {
            const string accessTokenPrefix = "Bearer ";
            if (accessToken.StartsWith(accessTokenPrefix))
            {
                accessToken = accessToken.Substring(accessTokenPrefix.Length);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:Key"])),
                ValidIssuer = configuration["JwtConfig:Issuer"],
                ValidAudience = configuration["JwtConfig:Audience"],
                ClockSkew = TimeSpan.Zero // optional: no tolerance for token expiration
            };

            if (!tokenHandler.CanReadToken(accessToken))
            {
                throw new APIException((int)HttpStatusCode.BadRequest, "Invalid access token");
            }

            var jwtToken = tokenHandler.ReadJwtToken(accessToken);
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                throw new APIException((int)HttpStatusCode.Unauthorized, ResponseCodeConstants.JWT_TOKEN_EXPIRED);
            }

            ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out _);
            if (claimsPrincipal == null)
            {
                throw new APIException((int)HttpStatusCode.BadRequest, "Failed to decrypt/validate access token");
            }

            currentLoginAccountDTO.Id = Guid.Parse(claimsPrincipal.FindFirstValue("UserId"));
            currentLoginAccountDTO.Email = claimsPrincipal.FindFirstValue("Email");
            currentLoginAccountDTO.FullName = claimsPrincipal.FindFirstValue("FullName");
            currentLoginAccountDTO.RoleName = claimsPrincipal.FindFirstValue("Role");

            return claimsPrincipal;
        }

    }
}
