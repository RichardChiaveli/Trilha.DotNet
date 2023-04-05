using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Trilha.DotNet.DependencyResolver
{
    public static class HstsResolver
    {
        public static IServiceCollection AddHstsAge(this IServiceCollection services)
        {
            var date = DateTime.Now.Date;
            
            services.AddHsts(opt =>
            {
                opt.MaxAge = date.AddYears(1) - date;
                opt.IncludeSubDomains = true;
            });

            return services;
        }
    }
}
