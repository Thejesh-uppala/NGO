using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NGO.Common;
using NGO.Repository.Contracts;

namespace NGO.Repository.NGO.Sql.Infrastructure
{
    public static class SQLDependencyRegistry
    {
        public static void DependencyRegistry(this IServiceCollection serviceCollection, AppSettings appsettings)
        {
            serviceCollection.AddDbContext<NGOContext>(options =>
            {
                options.UseSqlServer(appsettings.ConnectionString);
            });
            serviceCollection.AddTransient<IUserRepository, UserRepository>();
            serviceCollection.AddTransient<IUserDetailRepository, UserDetailRepository>();
            serviceCollection.AddTransient<IUserRolesRepository, UserRoleRepository>();
            serviceCollection.AddTransient<IRoleRepository, RoleRepository>();
            serviceCollection.AddTransient<IChildrensRepository, ChildrensRepository>();
            serviceCollection.AddTransient<IPaymentRepository, PaymentRepository>();
            serviceCollection.AddTransient<IUserAddressRepository, UserAddressRepository>();
            serviceCollection.AddTransient<IOrgRepository, OrgRepository>();
            serviceCollection.AddTransient<IOrgChapterRepository, OrgChapterRepository>();
            serviceCollection.AddTransient<IMembershipUserTypesRepository, MembershipUserTypesRepository>();
            serviceCollection.AddTransient<IEventDataRepository, EventDataRepository>();
            serviceCollection.AddTransient<IMatrimonialDataRepository, MatrimonialDataRepository>();
            serviceCollection.AddTransient<IMatrimonialPhotoDataRepository, MatrimonialPhotoDataRepository>();
        }
    }
}
