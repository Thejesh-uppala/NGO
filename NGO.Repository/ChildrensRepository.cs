using Microsoft.EntityFrameworkCore;
using NGO.Common.Models;
using NGO.Data;
using NGO.Model;
using NGO.Repository.Contracts;
using NGO.Repository.Infrastructure;
using Npgsql; // Use Npgsql for PostgreSQL DB access
using System.Data;

namespace NGO.Repository
{
    public class ChildrensRepository : Repository<ChildrensDetail>, IChildrensRepository
    {
        private readonly NGOContext _nGOContext;

        public ChildrensRepository(NGOContext nGOContext, ApplicationContext applicationContext)
            : base(nGOContext, applicationContext)
        {
            _nGOContext = nGOContext;
        }

        public async Task<UserDetailModel> GetCurrentUserDetails(int id)
        {
            UserDetailModel userDetailModel = new UserDetailModel();
            using (var connection = _nGOContext.Database.GetDbConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "UspGetCurrentUserDetails";  // Ensure procedure name matches PostgreSQL casing
                command.CommandType = CommandType.StoredProcedure;

                // Use NpgsqlParameter instead of SqlParameter for PostgreSQL
                command.Parameters.Add(new NpgsqlParameter("@userIdP", id));

                var reader = await command.ExecuteReaderAsync();

                // Read user details first
                if (reader.Read())
                {
                    userDetailModel.OrgId = reader.GetString(reader.GetOrdinal("OrgId"));
                    userDetailModel.UserId = reader.GetInt32(reader.GetOrdinal("UserId"));
                    userDetailModel.Address = reader.GetString(reader.GetOrdinal("Address"));
                    userDetailModel.State = reader.GetString(reader.GetOrdinal("State"));
                }

                // Move to the next result for children's details
                await reader.NextResultAsync();

                List<ChildrensDetailsModel> childrensDetailsModels = new List<ChildrensDetailsModel>();
                while (reader.Read())
                {
                    ChildrensDetailsModel childrensDetails = new ChildrensDetailsModel
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        ResidentCity = reader.GetString(reader.GetOrdinal("ResidentCity")),
                        UserDetailId = reader.GetInt32(reader.GetOrdinal("UserDetailId")),
                        ResidentState = reader.GetString(reader.GetOrdinal("ResidentState"))
                    };

                    childrensDetailsModels.Add(childrensDetails);
                }

                userDetailModel.ChildrensDetails = childrensDetailsModels;
            }

            return userDetailModel;
        }
    }
}
