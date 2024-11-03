using NGO.Data;

namespace NGO.Repository.Contracts
{
    public interface IRoleRepository:IRepository<Role>
    {
        Task<Role> GetRoleByNameAsync(string roleName);
    }
}
