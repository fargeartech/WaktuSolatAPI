using Microsoft.Extensions.DependencyInjection;
using WaktuSolatMY.Application.Services;

namespace WaktuSolatMY.Application.Extensions
{
    public static class ApplicationExtension
    {
        public static IServiceCollection AddApplications(this IServiceCollection services)
        {
            services.AddSingleton<IJakimSolatService, JakimSolatService>();
            return services;
        }
    }
}
