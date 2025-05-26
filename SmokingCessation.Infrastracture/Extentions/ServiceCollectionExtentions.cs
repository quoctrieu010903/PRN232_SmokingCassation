
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Infrastracture.Data;
using SmokingCessation.Infrastracture.Data.Persistence;
using SmokingCessation.Infrastracture.Repository;
using SmokingCessation.Infrastracture.Seeder;




namespace SmokingCessation.Infrastracture.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            
            var connectionString = configuration.GetConnectionString("SmokingCessationDB");
            services.AddDbContext<SmokingCassationDBContext>(options =>
                options.UseSqlServer(connectionString).EnableSensitiveDataLogging());


            services.AddScoped<ISmokingSessationSeeder, SmokingCessationSeeder>();
            // Dependency Injection 
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();



            ////
            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //         .AddEntityFrameworkStores<SmokingCassationDBContext>()
            //         .AddDefaultTokenProviders();

        }
    }
}
