
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SmokingCessation.Application.Service.Implementations;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Domain.Entities;



namespace SmokingCessation.Application.Extensions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            #region Add Scoped 
            services.AddScoped<IUserContext, UserContext>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            #endregion

            return services;
        }
    }
}
