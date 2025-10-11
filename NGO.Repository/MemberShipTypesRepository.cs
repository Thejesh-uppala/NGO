using Microsoft.EntityFrameworkCore;
using NGO.Common.Models;
using NGO.Data;
using NGO.Repository.Contracts;
using NGO.Repository.Infrastructure;

namespace NGO.Repository
{
    public class MembershipUserTypesRepository : Repository<MemberShipType>, IMembershipUserTypesRepository
    {
        private readonly NGOContext _nGOContext;
        public MembershipUserTypesRepository(NGOContext nGODbContext, ApplicationContext applicationContext) : base(nGODbContext, applicationContext)
        {
            _nGOContext = nGODbContext;
        }
        public async Task<MembershipData> GetUserMembershipDataAsync(int orgId, int MemberShipId)
        {
            return await _nGOContext.MembershipData
                .Where(m => m.OrgId == orgId && m.Id == MemberShipId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<MembershipData>> GetOrgMembershipDataAsync(int orgId)
        {
            return await _nGOContext.MembershipData
                .Where(o => o.OrgId == orgId).ToListAsync();
        }
    }
}
