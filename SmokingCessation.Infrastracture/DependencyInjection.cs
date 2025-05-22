
using Microsoft.Extensions.DependencyInjection;

namespace SmokingCessation.Infrastracture
{
    public static class DependencyInjection
    {
        /// <summary>
        /// This class is to configure services for infrastructure layer
        /// </summary>
       
        public static IServiceCollection AddInfrastractureDI(this IServiceCollection service)
        {
           
            return service;
        }
    }
}
