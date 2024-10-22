using NGO.Common.Models;
using NGO.Data;
using NGO.Repository.Contracts;
using NGO.Repository.Infrastructure;

namespace NGO.Repository
{
    public class UserDetailRepository : Repository<UserDetail>, IUserDetailRepository
    {
        private readonly NGOContext _nGOContext;
        public UserDetailRepository(NGOContext nGOContext, ApplicationContext applicationContext) : base(nGOContext, applicationContext)
        {
            _nGOContext = nGOContext;
        }

    }
}
