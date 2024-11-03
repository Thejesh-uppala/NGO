using Microsoft.EntityFrameworkCore;
using NGO.Common.Models;
using NGO.Data;
using NGO.Repository.Contracts;
using NGO.Repository.Infrastructure;

namespace NGO.Repository
{
    public class OrgRepository:Repository<Organization>, IOrgRepository
    {
        private readonly NGOContext _nGOContext;
        public OrgRepository(NGOContext nGOContext, ApplicationContext applicationContext) : base(nGOContext, applicationContext)
        {
            _nGOContext = nGOContext;
        }

        public async Task<Organization> GetByIdAsync(int orgId)
        {
            return await _nGOContext.Organizations
                .FirstOrDefaultAsync(o => o.Id == orgId);
        }
    }
}
