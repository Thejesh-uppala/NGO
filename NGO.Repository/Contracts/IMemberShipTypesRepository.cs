using NGO.Data;

namespace NGO.Repository.Contracts
{
    public interface IMembershipUserTypesRepository : IRepository<MemberShipType>
    {
        Task<MembershipData> GetUserMembershipDataAsync(int orgId, int MemberShipId);
        Task<List<MembershipData>> GetOrgMembershipDataAsync(int orgId);

    }
}
