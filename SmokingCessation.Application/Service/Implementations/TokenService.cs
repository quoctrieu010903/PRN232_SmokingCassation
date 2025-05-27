
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

        public TokenService(IConfiguration configuration, SymmetricAlgorithm secretkey, string? validIssuer, string? validAudience, int? expires, UserManager<ApplicationUser> userManager, ILogger<TokenService> logger)
        {
            var jwtSettings = configuration.GetSection("JwtConfig").Get<JwtSettings>();
            if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey))
            {
                throw new InvalidOperationException("JWT secret key is not configured. ");
            }
          
           
            _secretkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));


            _validIssuer = jwtSettings.Issuer;
            _validAudience = jwtSettings.Audience;
            _expires = jwtSettings.AccessTokenExpirationMinutes;
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
          var claims = new List<Claim>();
           {
                new Claim(ClaimTypes.Name, user?.UserName ?? String.Empty);
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString());
                new Claim(ClaimTypes.Email, user?.Email);
                new Claim("FirstName", user.FullName);
           };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
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
