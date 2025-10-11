using NGO.Common.Models;
using NGO.Data;
using NGO.Repository.Contracts;
using NGO.Repository.Infrastructure;

namespace NGO.Repository
{
    public class UserAddressRepository : Repository<Address>, IUserAddressRepository
    {
        private readonly NGOContext _nGOContext;

        public UserAddressRepository(NGOContext nGOContext, ApplicationContext applicationContext) : base(nGOContext, applicationContext)
        {
            _nGOContext = nGOContext;
        }

    }
}
