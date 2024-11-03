using NGO.Data;
using NGO.Model;
using NGO.Repository.Contracts;

namespace NGO.Repository
{
    public interface IUserRepository:IRepository<User>
    {
        Task<User> GetUserDetails(LoginModel loginModel);
        Task<User> CreateUser(string userName, string email, string password);
        Task<List<User>> GetUserByEmailAndOrg(string email, int orgId);

    }
}
