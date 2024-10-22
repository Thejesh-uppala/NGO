using Aykan.SRM.Model;
using NGO.Data;
using NGO.Model;

namespace NGO.Repository.Contracts
{
    public interface IChildrensRepository:IRepository<ChildrensDetail>
    {
        Task<UserDetailModel> GetCurrentUserDetails(int id);
    }
}
