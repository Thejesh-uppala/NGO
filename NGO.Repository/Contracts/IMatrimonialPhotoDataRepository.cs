
using NGO.Data;
namespace NGO.Repository.Contracts
{
    public interface IMatrimonialPhotoDataRepository : IRepository<MatrimonialDataPhoto>
    {
        Task<int> BulkAddMatrimonialDataAsync(List<MatrimonialDataPhoto> matrimonialPhotoEntities);
        Task<int> CreateMatrimonialDataAsync(MatrimonialDataPhoto data);
    }

}



