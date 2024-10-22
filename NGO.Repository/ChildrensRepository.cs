using Microsoft.EntityFrameworkCore;
using NGO.Common.Models;
using NGO.Data;
using NGO.Model;
using NGO.Repository.Contracts;
using NGO.Repository.Infrastructure;
using System.Data;

namespace NGO.Repository
{
    public class ChildrensRepository : Repository<ChildrensDetail>, IChildrensRepository
    {
        private readonly NGOContext _nGOContext;
        public ChildrensRepository(NGOContext nGOContext, ApplicationContext applicationContext) : base(nGOContext, applicationContext)
        {
            _nGOContext = nGOContext;
        }

        public async Task<UserDetailModel> GetCurrentUserDetails(int id)
        {
            //var userId = new SqlParameter("@p0", id);
            //var sql = "exec UspGetCurrentUserDetails @p0";
            //var result = _nGOContext.UspUserReturnModels.FromSqlRaw(sql, userId).ToList().FirstOrDefault();
            UserDetailModel userDetailModel = new UserDetailModel();
            using (var connection = _nGOContext.Database.GetDbConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "UspGetCurrentUserDetails";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@userId", id));
                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    userDetailModel.OrgId = reader["OrgId"].ToString();
                    userDetailModel.UserId = int.Parse(reader["UserId"].ToString());
                    userDetailModel.Address = reader["Address"].ToString();
                    userDetailModel.State = reader["State"].ToString();

                }
                await reader.NextResultAsync();
                List<ChildrensDetailsModel> childrensDetailsModels = new List<ChildrensDetailsModel>();
                while (reader.Read())
                {
                    ChildrensDetailsModel childrensDetails = new ChildrensDetailsModel();

                    childrensDetails.Id = int.Parse(reader["Id"].ToString());
                    childrensDetails.ResidentCity = reader["ResidentCity"].ToString();
                    childrensDetails.UserDetailId = int.Parse(reader["UserDetailId"].ToString());
                    childrensDetails.ResidentState = reader["ResidentState"].ToString();
                    childrensDetailsModels.Add(childrensDetails);
                }
                userDetailModel.ChildrensDetails = childrensDetailsModels;
            }
            return userDetailModel;
        }
    }
}
