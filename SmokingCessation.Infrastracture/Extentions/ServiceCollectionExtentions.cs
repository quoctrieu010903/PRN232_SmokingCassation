
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Infrastracture.Data;
using SmokingCessation.Infrastracture.Data.Persistence;
using SmokingCessation.Infrastracture.Photos;
using SmokingCessation.Infrastracture.Repository;
using SmokingCessation.Infrastracture.Seeder;




namespace SmokingCessation.Infrastracture.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            
            var connectionString = configuration.GetConnectionString("SmokingCessationDB");
            services.AddDbContext<SmokingCassationDBContext>(options =>
                options.UseSqlServer(connectionString).EnableSensitiveDataLogging());


            services.AddScoped<ISmokingSessationSeeder, SmokingCessationSeeder>();
            // Dependency Injection 
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                     .AddEntityFrameworkStores<SmokingCassationDBContext>()
                     .AddDefaultTokenProviders();

            
            // Add Auto Mapper
            



            return services;

        }
    }
}
