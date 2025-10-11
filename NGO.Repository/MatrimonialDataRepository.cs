using Microsoft.EntityFrameworkCore;
using NGO.Common.Models;
using NGO.Data;
using NGO.Repository.Contracts;
using NGO.Repository.Infrastructure;

namespace NGO.Repository.NGO.Sql
{
    public class MatrimonialDataRepository : Repository<MatrimonialData>, IMatrimonialDataRepository
    {
        private readonly NGOContext _context;
        public MatrimonialDataRepository(NGOContext nGOContext, ApplicationContext applicationContext) : base(nGOContext, applicationContext)
        {
            _context = nGOContext;
        }

        public async Task<MatrimonialData> GetMatrimonialDataByIdAsync(int id, int orgId)
        {
            return await _context.MatrimonialData
                      .Include(m => m.MatrimonialDataPhotos)
                    .FirstOrDefaultAsync(m => m.Id == id && m.OrgId == orgId);
        }
        public async Task<IEnumerable<MatrimonialData>> GetAllMatrimonialDataAsync(int orgId)
        {
            return await _context.MatrimonialData
              .Include(m => m.MatrimonialDataPhotos)
              .Where(m => m.OrgId == orgId)
              .ToListAsync();
        }

        public async Task<(List<MatrimonialData> Data, int TotalRecords)> GetFilteredMatrimonialDataAsync(
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
int pageSize)
        {
            var query = _context.MatrimonialData
                .Include(m => m.MatrimonialDataPhotos)
                .Where(m => m.OrgId == orgId && m.Status == "Approved" && m.RecordStatus != "expired");

            if (!string.IsNullOrEmpty(gender))
                query = query.Where(m => m.Gender.ToLower() == gender.ToLower());
            if (minAge.HasValue && maxAge.HasValue)
            {
                var minBirthYear = (DateTime.Today.Year - minAge.Value).ToString();
                var maxBirthYear = (DateTime.Today.Year - maxAge.Value).ToString();

                query = query.Where(m => m.BrideGroomDOB.Substring(0, 4).CompareTo(maxBirthYear) >= 0
                                      && m.BrideGroomDOB.Substring(0, 4).CompareTo(minBirthYear) <= 0);
            }
            else if (maxAge.HasValue)
            {
                var maxBirthYear = (DateTime.Today.Year - maxAge.Value).ToString();
                query = query.Where(m => m.BrideGroomDOB.Substring(0, 4).CompareTo(maxBirthYear) >= 0);
            }
            else if (minAge.HasValue)
            {
                var minBirthYear = (DateTime.Today.Year - minAge.Value).ToString();
                query = query.Where(m => m.BrideGroomDOB.Substring(0, 4).CompareTo(minBirthYear) <= 0);
            }




            if (!string.IsNullOrWhiteSpace(minHeight) && decimal.TryParse(minHeight, out decimal minHeightValue))
            {
                query = query.Where(m => m.BrideGroomHeight >= minHeightValue);
            }

            if (!string.IsNullOrWhiteSpace(maxHeight) && decimal.TryParse(maxHeight, out decimal maxHeightValue))
            {
                query = query.Where(m => m.BrideGroomHeight <= maxHeightValue);
            }



            if (!string.IsNullOrEmpty(state))
                query = query.Where(m => m.BrideGroomCurrentState.ToLower().Contains(state.ToLower()));
            if (!string.IsNullOrEmpty(country))
                query = query.Where(m => m.BrideGroomCurrentCountry.ToLower().Contains(country.ToLower()));
            if (!string.IsNullOrEmpty(education))
                query = query.Where(m => m.BrideGroomEducation.ToLower().Contains(education.ToLower()));
            if (!string.IsNullOrEmpty(profession))
                query = query.Where(m => m.BrideGroomEmploymentInfo.ToLower().Contains(profession.ToLower()));
            if (!string.IsNullOrEmpty(prevMarried))
                query = query.Where(m => m.BrideGroomPrevMarried.ToLower() == prevMarried.ToLower());
            if (!string.IsNullOrEmpty(searchValue))
            {
                var searchLower = searchValue.ToLower();
                query = query.Where(m =>
                    (m.BrideOrGroomFName + " " + m.BrideOrGroomLName).ToLower().Contains(searchLower) ||
                    m.BrideGroomCurrentLocation.ToLower().Contains(searchLower) ||
                    m.BrideGroomEmploymentInfo.ToLower().Contains(searchLower)
                );
            }

            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalRecords);
        }

        public async Task<(List<MatrimonialData> Data, int TotalRecords)> GetAllApprovedMatrimonialDataAsync(int orgId,
    int page,
    int pageSize)
        {
            var query = _context.MatrimonialData
                .Include(m => m.MatrimonialDataPhotos)
                .Where(m => m.OrgId == orgId && m.Status == "Approved" && m.RecordStatus != "expired");

            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalRecords);
        }

        public async Task<List<MatrimonialData>> GetProfilesByIdsAsync(int orgId, List<int> userIds)
        {
            return await _context.MatrimonialData
                .Where(profile => userIds.Contains(profile.Id) && profile.OrgId == orgId)
                .ToListAsync();
        }

        public async Task<int> CreateMatrimonialDataAsync(MatrimonialData data)
        {
            _context.MatrimonialData.Add(data);
            return await _context.SaveChangesAsync();
        }






    }

}


