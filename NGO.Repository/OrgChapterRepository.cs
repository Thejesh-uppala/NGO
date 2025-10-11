using Microsoft.EntityFrameworkCore;
using NGO.Common.Models;
using NGO.Data;
using NGO.Repository.Contracts;
using NGO.Repository.Infrastructure;

namespace NGO.Repository
{
    public class OrgChapterRepository : Repository<OrganizationChapter>, IOrgChapterRepository
    {
        private readonly NGOContext _nGOContext;
        public OrgChapterRepository(NGOContext nGODbContext, ApplicationContext applicationContext) : base(nGODbContext, applicationContext)
        {
            _nGOContext = nGODbContext;
        }
        public async Task<List<OrganizationChapter>> GetOrgChaptersAsync(int orgId)
        {
            return await _nGOContext.OrganizationChapters
                .Where(o => o.OrgId == orgId).ToListAsync();
        }


    }
}
