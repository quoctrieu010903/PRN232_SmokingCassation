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
                var path = context.Request.Path.Value.ToLower();
                var method = context.Request.Method.ToUpper();
                //if (!((path == "/api/v1/auth" && method == "POST") || path == "/api/v1/auth/sign-in-with-refresh-token"))
                //{
                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    var currentLoginAccountDTO = context.RequestServices.GetRequiredService<CurrentUserResponse>();
                    var configuration = context.RequestServices.GetRequiredService<IConfiguration>();

                    var userClaims = currentLoginAccountDTO.DecryptAccessToken(context.Request.Headers["Authorization"], configuration);

                
                     context.User = userClaims; // 👈 RẤT QUAN TRỌNG
                 }
                //}
                await _next(context);
            }
    }
}
