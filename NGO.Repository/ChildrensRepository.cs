using NGO.Common.Models;
using NGO.Data;
using NGO.Repository.Contracts;
using NGO.Repository.Infrastructure;

namespace NGO.Repository
{
    public class ChildrensRepository : Repository<ChildrensDetail>, IChildrensRepository
    {
        private readonly NGOContext _nGOContext;

        public ChildrensRepository(NGOContext nGOContext, ApplicationContext applicationContext)
            : base(nGOContext, applicationContext)
        {
            _nGOContext = nGOContext;
        }
    }
}
