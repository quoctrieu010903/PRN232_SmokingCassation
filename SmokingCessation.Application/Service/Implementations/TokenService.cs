    
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Base;
using SmokingCessation.Core.Utils;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Application.Service.Implementations
{
    public class TokenService : ITokenService
    {
        
     
        private readonly SymmetricSecurityKey _secretkey;
        private readonly string? _validIssuer;
        private readonly string? _validAudience;
        private readonly int? _expires;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IOptions<JwtSettings> options,
                       UserManager<ApplicationUser> userManager,
                       ILogger<TokenService> logger)
                        
        {
            var jwtSettings = options.Value;

            if (string.IsNullOrEmpty(jwtSettings.Key))
            {
                throw new InvalidOperationException("JWT secret key is not configured.");
            }

            _secretkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            _validIssuer = jwtSettings.Issuer;
            _validAudience = jwtSettings.Audience;
            _expires = jwtSettings.TokenValidityMins;
            _userManager = userManager;
            _logger = logger;
        }
        

        public  string GenerateRefeshToken()
        {
           var ramdomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();

            var refeshToken = Convert.ToBase64String(ramdomNumber);
            return refeshToken;
        }

        private async Task<List<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
                {

                    new Claim("Email", user.Email),
                    new Claim("FullName", user.FullName),
                    new Claim("UserName", user.UserName), // chuẩn nhất cho ID
                    new Claim("UserId" , user.Id.ToString())

                };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim("Role", role)));

            return claims;
        }



        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        => new JwtSecurityToken(
                issuer: _validIssuer,
                audience: _validAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes((double)_expires),
                signingCredentials: signingCredentials
            );

        public async Task<string> GenerateToken(ApplicationUser user)
        {
            var signgingCredentials = new SigningCredentials(_secretkey, SecurityAlgorithms.HmacSha256);
            var claims = await GetClaimsAsync(user);
            var tokenOptions= GenerateTokenOptions(signgingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        }
      

    }
}
