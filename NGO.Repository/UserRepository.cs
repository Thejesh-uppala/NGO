using System.Data; // For DataTable
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using NGO.Common.Models;
using NGO.Data;
using NGO.Model;
using NGO.Repository.Infrastructure;
using Npgsql;

namespace NGO.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly NGOContext _nGOContext;
        public UserRepository(NGOContext nGOContext, ApplicationContext applicationContext) : base(nGOContext, applicationContext)
        {
            _nGOContext = nGOContext;
        }



        public async Task<List<User>> GetUsersByOrg(int orgId)
        {
            return await _nGOContext.Users.Include(u => u.UserRoles)
                                                .ThenInclude(ur => ur.Role).Where(u => u.UserOrganizations.Any(o => o.OrganizationId == orgId)).ToListAsync();
        }


        public async Task DeleteUsersAsync(List<int> userIds, int updatedBy, int orgId)
        {
            if (userIds == null || !userIds.Any())
                throw new ArgumentException("UserIds cannot be null or empty.");

            // Fetch users based on the provided UserIds and OrgId
            var users = await _nGOContext.Users
            .Where(u => userIds.Contains(u.Id) && u.UserOrganizations.Any(o => o.OrganizationId == orgId) && !u.IsDeleted)
            .ToListAsync();

            if (users.Any())
            {
                // Iterate over users and mark them as deleted
                foreach (var user in users)
                {
                    user.IsDeleted = true;
                    user.UpdatedBy = updatedBy; // Set the user who performed the delete
                    user.UpdatedOn = DateTime.UtcNow; // Set the update timestamp
                }

                // Bulk update the users
                _nGOContext.Users.UpdateRange(users);
                await _nGOContext.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("No users found for the given UserIds and OrgId.");
            }
        }

        //        var userDetails = await _nGOContext.UserDetails
        //    .Where(u => userIds.Contains(u.UserId) &&
        //                u.OrgId == orgId &&
        //                !_nGOContext.Users.Any(x => x.Id == u.UserId && x.IsDeleted))
        //    .ToListAsync();

        //foreach (var user in userDetails)
        //{
        //    user.IsApproved = "Approved";
        //    user.ApprovedBy = actionUserId;
        //}
        //    await _nGOContext.SaveChangesAsync();


        public async Task<bool> AcceptMemberships(int orgId, List<int> userIds, int actionUserId)
        {
            if (userIds == null || !userIds.Any())
                throw new ArgumentException("UserIds cannot be null or empty.", nameof(userIds));

            string userIdsString = string.Join(",", userIds);

            string sqlQuery = @"
        UPDATE ""UserDetails""
        SET ""IsApproved"" = @AcceptedStatus,
            ""ApprovedBy"" = @ActionUserId
        WHERE ""UserId"" IN (
            SELECT CAST(value AS INTEGER)
            FROM unnest(string_to_array(@UserIds, ',')) AS value
        )
        AND ""UserId"" IN (
            SELECT ""UserId"" FROM ""UserOrganization"" WHERE ""OrganizationId"" = @OrgId
        )
        AND ""UserId"" IN (
            SELECT ""Id"" FROM ""Users"" WHERE ""IsDeleted"" = FALSE
        )";

            var parameters = new[]
            {
        new NpgsqlParameter("@AcceptedStatus", "Approved"),
        new NpgsqlParameter("@UserIds", userIdsString),
        new NpgsqlParameter("@OrgId", orgId),
        new NpgsqlParameter("@ActionUserId", actionUserId)
    };

            int affectedRows = await _nGOContext.Database.ExecuteSqlRawAsync(sqlQuery, parameters);
            return affectedRows > 0;
        }


        public async Task<bool> RejectMemberships(int orgId, List<int> userIds, int actionUserId)
        {
            if (userIds == null || !userIds.Any())
                throw new ArgumentException("UserIds cannot be null or empty.", nameof(userIds));

            string userIdsString = string.Join(",", userIds);

            string sqlQuery = @"
        UPDATE ""UserDetails""
        SET ""IsApproved"" = @RejectedStatus,
            ""ApprovedBy"" = @ActionUserId
        WHERE ""UserId"" IN (
            SELECT CAST(value AS INTEGER)
            FROM unnest(string_to_array(@UserIds, ',')) AS value
        )
        AND ""UserId"" IN (
            SELECT ""UserId"" FROM ""UserOrganization"" WHERE ""OrganizationId"" = @OrgId
        )
        AND ""UserId"" IN (
            SELECT ""Id"" FROM ""Users"" WHERE ""IsDeleted"" = FALSE
        )";

            var parameters = new[]
            {
        new NpgsqlParameter("@RejectedStatus", "Rejected"),
        new NpgsqlParameter("@UserIds", userIdsString),
        new NpgsqlParameter("@OrgId", orgId),
        new NpgsqlParameter("@ActionUserId", actionUserId)
    };

            int affectedRows = await _nGOContext.Database.ExecuteSqlRawAsync(sqlQuery, parameters);
            return affectedRows > 0;
        }


        public async Task<User> GetUserByIdAndOrg(int userId, int orgId)
        {
            return await _nGOContext.Users
                .Include(u => u.UserOrganizations)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted && u.UserOrganizations.Any(o => o.OrganizationId == orgId));
        }

        public async Task<List<UserReminderDto>> GetUsersByEmailsAndOrg(List<string> emails, int orgId)
        {
            if (emails == null || !emails.Any())
                throw new ArgumentException("Email list cannot be null or empty.");

            return await (from user in _nGOContext.Users
                          join userDetails in _nGOContext.UserDetails on user.Id equals userDetails.UserId
                          where emails.Contains(user.Email)
                                && !user.IsDeleted
                                && userDetails.OrgId == orgId
                          select new UserReminderDto
                          {
                              FirstName = userDetails.FirstName,
                              LastName = userDetails.LastName,
                              MembershipValidityDate = userDetails.MembershipValidityDate,
                              MembershipStatus = userDetails.MembershipStatus,
                              Email = user.Email
                          }).ToListAsync();
        }



        public async Task<object> GetEmailsByUserIdsAsync(List<int> userIds)
        {
            if (userIds == null || !userIds.Any())
                throw new ArgumentException("UserIds cannot be null or empty.");

            var emails = await _nGOContext.Users
                .Where(u => userIds.Contains(u.Id) && !u.IsDeleted)
                .Select(u => u.Email)
                .ToListAsync();

            return userIds.Count == 1 ? emails.FirstOrDefault() : emails;
        }


        public async Task<List<User>> GetUserByEmailAndOrg(string email, int orgId)
        {
            return await _nGOContext.Users
                .Include(u => u.UserOrganizations) // Load the organizations the user is associated with
                .Include(u => u.UserRoles.Where(ur => ur.OrgId == orgId))
                    .ThenInclude(ur => ur.Role)     // Load each role's details for the user's roles
                .Where(u => u.Email == email && !u.IsDeleted &&
                            u.UserOrganizations.Any(o => o.OrganizationId == orgId))
                .ToListAsync();
        }

        public async Task UpdateUser(User user)
        {
            _nGOContext.Users.Update(user);
            await _nGOContext.SaveChangesAsync();
        }



    }
}
