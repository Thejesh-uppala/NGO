using NGO.Data;
using NGO.Model;
using NGO.Repository.Contracts;

namespace NGO.Repository
{
    public interface IUserRepository:IRepository<User>
    {
        Task<User> CreateUser(string userName, string email, string password);
        Task<List<User>> GetUserByEmailAndOrg(string email, int orgId);
        Task<User?> GetUserByRefreshTokenAndUserId(Guid refreshToken, int userId);
        Task<User> GetUserByIdAndOrg(int userId, int orgId);
    }
}
