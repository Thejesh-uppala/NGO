using NGO.Data;
using NGO.Model;

namespace NGO.Repository.Contracts
{
    public interface IOrganizationConfigService : IRepository<OrganizationConfiguration>
    {
        Task<OrganizationConfiguration> GetConfigByOrgId(int orgId);
        Task SaveConfigAsync(OrganizationConfigInputModel inputModel);

    }

}
