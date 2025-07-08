using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmokingCessation.Application.Helpers;
using SmokingCessation.Application.Mapping;
using SmokingCessation.Application.Service.Implementations;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Application.Service.Interfaces;
using SmokingCessation.Core.Base;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Infrastracture.Photos;
using static SmokingCessation.Application.DTOs.Response.AuthenticationResponse;



namespace SmokingCessation.Application.Extensions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            #region Add Scoped 
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddScoped<JWTHelper>();


            services.AddScoped<CurrentUserResponse>();



            services.Configure<JwtSettings>(configuration.GetSection("JwtConfig"));
            services.Configure<CloundinarySettings>(configuration.GetSection("Cloudinary"));
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IMemberShipPackage, MemberShipPackService>();
            services.AddScoped<IQuitPlanService, QuitPlanService>();
            services.AddScoped<IProgressLogsService, ProgressLogsService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IRankingService, RankingService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IUserPackageService, UserPackageService>();
            services.AddScoped<IDeepSeekService, DeepSeekService>();
            services.AddScoped<ICoachAdviceLogService, CoachAdviceLogService>();
            services.AddScoped<IAchivementServie, AchivementService>();
            services.AddScoped<IUserAchievementService, UserAchievementService>();
            services.AddScoped<IVNPayService, VNPaymentService>();
            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            services.AddScoped<IUserDashboardService, UserDashboardService>();


            #endregion

            return services;
        }
    }
}
