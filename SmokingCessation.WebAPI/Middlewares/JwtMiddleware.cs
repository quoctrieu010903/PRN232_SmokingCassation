using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SmokingCessation.Application.Helpers;
using SmokingCessation.WebAPI.Attributes;
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
                var endpoint = context.GetEndpoint();

                // Skip token validation if:
                // 1. The endpoint has [AllowAnonymous] attribute
                // 2. The endpoint doesn't have [Authorize] attribute
                // 3. The endpoint has [BypassJwtMiddleware] attribute
                if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null ||
                    endpoint?.Metadata?.GetMetadata<IAuthorizeData>() == null ||
                    endpoint?.Metadata?.GetMetadata<BypassJwtMiddlewareAttribute>() != null)
                {
                    await _next(context);
                    return;
                }

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
