using System.Security.Claims;
using SmokingCessation.Application.Helpers;
using static SmokingCessation.Application.DTOs.Response.AuthenticationResponse;

namespace SmokingCessation.WebAPI.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                _logger.LogInformation($"Processing request to: {context.Request.Path}");

                if (token != null)
                {
                    var currentLoginAccountDTO = context.RequestServices.GetRequiredService<CurrentUserResponse>();
                    var configuration = context.RequestServices.GetRequiredService<IConfiguration>();

                    var userClaims = currentLoginAccountDTO.DecryptAccessToken($"Bearer {token}", configuration);
                    if (userClaims != null)
                    {
                        // Get roles from both standard and custom claim types
                        var roles = userClaims.Claims
                            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role" || c.Type == "Role")
                            .Select(c => c.Value)
                            .Distinct();
                            
                        // Create a new ClaimsIdentity with all claims
                        var identity = new ClaimsIdentity(userClaims.Claims, "Bearer");
                        
                        // Add role claims to the identity
                        foreach (var role in roles)
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, role));
                        }
                        
                        context.User = new ClaimsPrincipal(identity);
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        return;
                    }
                }
                else
                {
                    context.Response.StatusCode = 401;
                    return;
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 404;
                return;
            }

            await _next(context);
        }
    }
}
