
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmokingCessation.Application.Helpers;
using SmokingCessation.Application.Mapping;
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
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddScoped<JWTHelper>();
           
          
                services.AddScoped<CurrentUserResponse>();
            

               
                services.Configure<JwtSettings>(configuration.GetSection("JwtConfig"));
            services.AddScoped<IUserContext, UserContext>();    
            services.AddScoped<ITokenService, TokenService>();
                services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IMemberShipPackage, MemberShipPackService>();
           services.AddScoped<IQuitPlanService, QuitPlanService>();
            services.AddScoped<IProgressLogsService , ProgressLogsService>();
            services.AddScoped<IBlogService, BlogService>();



            #endregion

            return services;
        }
    }
}
