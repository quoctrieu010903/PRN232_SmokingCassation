
using Microsoft.Extensions.DependencyInjection;
using SmokingCessation.Application.Service.Implementations;
using SmokingCessation.Application.Service.Interface;



namespace SmokingCessation.Application.Extensions
{
    public static class ServiceCollectionExtentions 
    {
        public static void  AddApplication(this IServiceCollection services)
        {
            #region Add Scoped 
            services.AddScoped<IUserContext,UserContext>();
            #endregion




            services.AddHttpContextAccessor();
        }
    }
}
