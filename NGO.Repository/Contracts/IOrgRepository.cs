using NGO.Data;

namespace NGO.Repository.Contracts
{
    public interface IOrgRepository : IRepository<Organization>
    {
        Task<Organization> GetByIdAsync(int orgId);
    }
}
