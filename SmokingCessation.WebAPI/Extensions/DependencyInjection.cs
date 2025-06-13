using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SmokingCessation.Application;
using SmokingCessation.Infrastracture;
using SmokingCessation.Infrastracture.Extentions;
using SmokingCessation.Core.Base;
using System.Text.Json;

namespace SmokingCessation.WebAPI.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services, IConfiguration configuration)
        {
            services
                    .AddJwtAuthentication(configuration)
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

            public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
            {
                var jwtSettings = configuration.GetSection("JwtConfig").Get<JwtSettings>();

                services
                    .AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;

                        options.TokenValidationParameters = new TokenValidationParameters
                        {

                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtSettings.Issuer,
                            ValidAudience = jwtSettings.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(jwtSettings.Key))
                        };

                        // Sửa lại phần xử lý sự kiện OnChallenge
                        options.Events = new JwtBearerEvents
                        {
                            OnChallenge = async context =>
                            {
                                // Tắt xử lý mặc định
                                context.HandleResponse();

                                // Kiểm tra nếu là API request
                                if (context.Request.Path.StartsWithSegments("/api"))
                                {
                                    context.Response.StatusCode = 401;
                                    context.Response.ContentType = "application/json";
                                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                                    {
                                        error = "Unauthorized",
                                        message = "Authentication token is required"
                                    }));
                                }
                                // Chuyển hướng cho MVC request
                                else
                                {
                                    context.Response.Redirect("/Account/Login");
                                }
                            }
                        };
                    });


                return services;
            }



    }
}
