using NGO.Data;

namespace NGO.Repository.Contracts
{
    public interface IOrgChapterRepository : IRepository<OrganizationChapter>
    {
        Task<List<OrganizationChapter>> GetOrgChaptersAsync(int orgId);
    }
}
