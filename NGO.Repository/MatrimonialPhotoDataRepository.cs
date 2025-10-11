using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NGO.Common.Models;
using NGO.Data;
using NGO.Repository.Contracts;
using NGO.Repository.Infrastructure;

namespace NGO.Repository.NGO.Sql
{
    public class MatrimonialPhotoDataRepository : Repository<MatrimonialDataPhoto>, IMatrimonialPhotoDataRepository
    {
        private readonly NGOContext _context;
        public MatrimonialPhotoDataRepository(NGOContext nGOContext, ApplicationContext applicationContext) : base(nGOContext, applicationContext)
        {
            _context = nGOContext;
        }

        //public async Task<int> BulkAddMatrimonialDataAsync(List<MatrimonialDataPhoto> matrimonialPhotoEntities)
        //{
        //    if (matrimonialPhotoEntities == null || matrimonialPhotoEntities.Count == 0)
        //        return 0;

        //    var dataTable = new DataTable();
        //    dataTable.Columns.Add("MatrimonailId", typeof(int));
        //    dataTable.Columns.Add("PhotoType", typeof(string));
        //    dataTable.Columns.Add("PhotoPath", typeof(string));

        //    foreach (var entity in matrimonialPhotoEntities)
        //    {
        //        dataTable.Rows.Add(entity.MatrimonailId, entity.PhotoType, entity.PhotoPath);
        //    }

        //    using (var connection = _context.Database.GetDbConnection() as SqlConnection)
        //    {
        //        await connection.OpenAsync();
        //        using (var command = new SqlCommand(
        //            "INSERT INTO ngoourgumpu.dbo.MatrimonialDataPhotos (MatrimonailId, PhotoType, PhotoPath) " +
        //            "SELECT MatrimonailId, PhotoType, PhotoPath FROM @Photos", connection))
        //        {
        //            var parameter = command.Parameters.AddWithValue("@Photos", dataTable);
        //            parameter.SqlDbType = SqlDbType.Structured;
        //            parameter.TypeName = "dbo.MatrimonialDataPhotoTableType";

        //            return await command.ExecuteNonQueryAsync();
        //        }
        //    }
        //}

        public async Task<int> BulkAddMatrimonialDataAsync(List<MatrimonialDataPhoto> matrimonialPhotoEntities)
        {
            // Add all entities to the context

            _context.MatrimonialDataPhotos.AddRange(matrimonialPhotoEntities);
            // Save changes to persist in the database
            return await _context.SaveChangesAsync();
        }


        public async Task<int> CreateMatrimonialDataAsync(MatrimonialDataPhoto data)
        {
            _context.MatrimonialDataPhotos.Add(data);
            return await _context.SaveChangesAsync();
        }




    }

}


