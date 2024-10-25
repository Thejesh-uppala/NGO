using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NGO.Common;
using NGO.Repository.Contracts;
using NGO.Repository.Infrastructure;

namespace NGO.Repository.NGO.Sql.Infrastructure
{
    public static class SQLDependencyRegistry
    {
        public static void DependencyRegistry(this IServiceCollection serviceCollection,AppSettings appsettings)
        {
            serviceCollection.AddDbContext<NGOContext>(options =>
            {
                options.UseNpgsql(appsettings.ConnectionString);
            });
            serviceCollection.AddTransient<IUserRepository, UserRepository>();
            serviceCollection.AddTransient<IUserDetailRepository, UserDetailRepository>();
            serviceCollection.AddTransient<IUserRolesRepository, UserRoleRepository>();
            serviceCollection.AddTransient<IRoleRepository, RoleRepository>();
            serviceCollection.AddTransient<IChildrensRepository, ChildrensRepository>();
            serviceCollection.AddTransient<IPaymentRepository, PaymentRepository>();
            serviceCollection.AddTransient<IOrgRepository, OrgRepository>();
            serviceCollection.AddTransient<IOrgChapterRepository, OrgChapterRepository>();
            serviceCollection.AddTransient<IMemberShipTypesRepository, MemberShipTypesRepository>();

        }
    }
}
