using NGO.Data;
namespace NGO.Repository.Contracts
{
    public interface IMatrimonialDataRepository : IRepository<MatrimonialData>
    {
        Task<MatrimonialData> GetMatrimonialDataByIdAsync(int id, int orgId);
        Task<IEnumerable<MatrimonialData>> GetAllMatrimonialDataAsync(int orgId);

        Task<int> CreateMatrimonialDataAsync(MatrimonialData data);
        Task<List<MatrimonialData>> GetProfilesByIdsAsync(int orgId, List<int> userIds);
        Task<(List<MatrimonialData> Data, int TotalRecords)> GetAllApprovedMatrimonialDataAsync(
    int orgId,
    int page,
    int pageSize);
        Task<(List<MatrimonialData> Data, int TotalRecords)> GetFilteredMatrimonialDataAsync(
            int orgId,
            string gender,
            int? minAge,
            int? maxAge,
            string? minHeight,
            string? maxHeight,
            string state,
            string country,
            string education,
            string profession,
            string prevMarried,
                 string searchValue,
            int page,
            int pageSize);

    }

}



