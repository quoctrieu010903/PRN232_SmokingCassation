

using Microsoft.Extensions.DependencyInjection;

namespace SmokingCessation.Application
{
    public static class DependencyInjection
    {
        /// <summary>
        /// This class is to configure services for application layer.
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationDI(this IServiceCollection service)
        {

            return service;
        }
    }
}
