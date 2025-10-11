using NGO.Data;
using NGO.Model;
using NGO.Repository.Contracts;

namespace NGO.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<List<UserReminderDto>> GetUsersByEmailsAndOrg(List<string> emails, int orgId);

        Task<List<User>> GetUserByEmailAndOrg(string email, int orgId);
        Task<List<User>> GetUsersByOrg(int orgId);
        Task<User> GetUserByIdAndOrg(int userId, int orgId);
        Task UpdateUser(User user);
        Task DeleteUsersAsync(List<int> userIds, int updatedBy, int orgId);

        Task<bool> AcceptMemberships(int orgId, List<int> userIds, int ActionUserId);
        Task<object> GetEmailsByUserIdsAsync(List<int> userIds);
        Task<bool> RejectMemberships(int orgId, List<int> userIds, int ActionUserId);
    }
}
