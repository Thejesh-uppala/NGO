using NGO.Common.Models;
using NGO.Data;
using NGO.Repository.Contracts;
using NGO.Repository.Infrastructure;

namespace NGO.Repository
{
    public class UserRoleRepository:Repository<UserRole>,IUserRolesRepository
    {
        private readonly NGOContext _nGOContext;
        public UserRoleRepository(NGOContext nGOContext, ApplicationContext applicationContext) : base(nGOContext, applicationContext)
        {
            _nGOContext = nGOContext;
        }
    }
}
