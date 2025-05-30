
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmokingCessation.Application.Helpers;
using SmokingCessation.Application.Service.Implementations;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Base;
using SmokingCessation.Domain.Entities;
using static SmokingCessation.Application.DTOs.Response.AuthenticationResponse;



namespace SmokingCessation.Application.Extensions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            #region Add Scoped 
                   services.AddScoped<JWTHelper>();
           
          
                services.AddScoped<CurrentUserResponse>();
                services.AddScoped<IUserContext, UserContext>();
                services.Configure<JwtSettings>(configuration.GetSection("JwtConfig"));
                services.AddScoped<ITokenService, TokenService>();
                services.AddScoped<IAuthenticationService, AuthenticationService>();


            #endregion

            return services;
        }
    }
}
