using SmokingCessation.Application;
using SmokingCessation.Infrastracture;
using SmokingCessation.Infrastracture.Extentions;

namespace SmokingCessation.WebAPI.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services)
        {
            services
                    .AddApplicationDI()
                    .ConfigCors();
                    
                    
            return services;
        }
        public static void ConfigCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder =>
                    {
                        builder.WithOrigins("*")      
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

        }

    }
}
