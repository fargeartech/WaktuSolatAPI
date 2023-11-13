using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
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

    public static class StringExtensions
    {
        private static readonly Regex _regex = new Regex("<.*?>");

        public static string StripTags(this string input)
        {
            return _regex.Replace(input, string.Empty);
        }
    }
}
