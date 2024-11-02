using Microsoft.EntityFrameworkCore;
using NGO.Common.Helpers;
using NGO.Common.Models;
using NGO.Data;
using NGO.Model;
using NGO.Repository.Infrastructure;
using System.Globalization;

namespace NGO.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly NGOContext _nGOContext;
        public UserRepository(NGOContext nGOContext, ApplicationContext applicationContext) : base(nGOContext, applicationContext)
        {
            _nGOContext = nGOContext;
        }
        public async Task<List<User>> GetUserByEmailAndOrg(string email, int orgId)
        {
            return await _nGOContext.Users
                .Include(u => u.UserOrganizations) 
                .Where(u => u.Email == email &&
                            u.UserOrganizations.Any(o => o.OrganizationId == orgId))
                .ToListAsync();
        }

        public async Task<User> GetUserDetails(LoginModel loginModel)
        {
            return await _nGOContext.Users.FirstOrDefaultAsync(u => u.Email == loginModel.UserName);
        }
        public async Task<User> CreateUser(string userName, string email, string password)
        {
            var createdDate = DateTime.Now;
            var user = new User()
            {
                Name = userName,
                CreatedOn = createdDate,
                Password = Cryptography.ComputeSHA256Hash(password, createdDate.ToString(CultureInfo.InvariantCulture)),
                Email = email
            };
            _nGOContext.Users.Add(user);
            try
            {
                await _nGOContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
          

            //var userRole = new UserRole()
            //{
            //    UserId = user.Id,
            //    RoleId = roleId
            //};
            //_nGOContext.UserRoles.Add(userRole);
            //await _nGOContext.SaveChangesAsync();

            return user;
        }
    }
}
