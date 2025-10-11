using Microsoft.Extensions.DependencyInjection;
using NGO.Common;
using NGO.Repository.NGO.Sql.Infrastructure;

namespace NGO.Business
{
    public static class BusinessDependencyRegistry
    {
        public static void RegisterDependency(this IServiceCollection services, AppSettings appSettings)
        {
            SQLDependencyRegistry.DependencyRegistry(services, appSettings);
            services.AddTransient<UserBusiness>();
            services.AddTransient<RegisterBusiness>();
            services.AddTransient<ReminderBusiness>();
            services.AddTransient<PasswordHandler>();
            services.AddTransient<EventBusiness>();
            services.AddTransient<MatrimonialBusiness>();
            services.AddTransient<CommonBusiness>();
            services.AddTransient<SMTPEmailProvider>();
        }
    }
}
