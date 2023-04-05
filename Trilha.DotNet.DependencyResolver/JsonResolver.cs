using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Trilha.DotNet.DependencyResolver
{
    public static class JsonResolver
    {
        public static IServiceCollection AddJsonMvc(this IServiceCollection services)
        {
            services.Configure<MvcJsonOptions>(opt =>
            {
                var settings = opt.SerializerSettings;

                settings.Formatting = Formatting.None;
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                settings.DefaultValueHandling = DefaultValueHandling.Include;
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            return services;
        }
    }
}
