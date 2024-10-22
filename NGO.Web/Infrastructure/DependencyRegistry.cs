using NGO.Business;
using NGO.Common;
using NGO.Common.Models;

namespace NGO.Web.Infrastructure
{
    public static class DependencyRegistry
    {
        public static void RegisterDependency(this IServiceCollection services,AppSettings appSettings)
        {
            services.AddSingleton<IHttpContextAccessor , HttpContextAccessor>();
            services.AddSingleton(appSettings);
            services.AddScoped<ApplicationContext>();
            services.AddMemoryCache();
            BusinessDependencyRegistry.RegisterDependency(services,appSettings); 
        }
    }
}
