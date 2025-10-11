using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NGO.Common.Models;
using NGO.Data;
using NGO.Model;
using NGO.Repository.Contracts;
using NGO.Repository.Infrastructure;

namespace NGO.Repository
{
    public class OrganizationConfigService : Repository<OrganizationConfiguration>, IOrganizationConfigService
    {
        private readonly NGOContext _context;
        private readonly IMemoryCache _cache;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public OrganizationConfigService(
            NGOContext context,
            IMemoryCache cache,
            IDataProtectionProvider dataProtectionProvider,
            ApplicationContext applicationContext)
            : base(context, applicationContext)
        {
            _context = context;
            _cache = cache;
            _dataProtectionProvider = dataProtectionProvider;
        }

        public async Task<OrganizationConfiguration> GetConfigByOrgId(int orgId)
        {
            string cacheKey = $"OrgConfig_{orgId}";

            // Try to get from cache
            if (_cache.TryGetValue(cacheKey, out OrganizationConfiguration? cachedConfig) && cachedConfig != null)
            {
                return cachedConfig;
            }

            // Fetch from database if not in cache
            var config = await _context.OrganizationConfigurations
                .AsNoTracking() // Optimize by not tracking entity
                .FirstOrDefaultAsync(c => c.OrgId == orgId);

            if (config == null)
            {
                throw new Exception($"No configuration found for OrgId: {orgId}");
            }

            // Set the DataProtectionProvider on the fetched config
            config.SetDataProtectionProvider(_dataProtectionProvider);

            // Cache the result
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1), // Cache for 1 hour
                SlidingExpiration = TimeSpan.FromMinutes(15) // Refresh if accessed within 15 minutes
            };
            _cache.Set(cacheKey, config, cacheOptions);

            return config;
        }



        public async Task SaveConfigAsync(OrganizationConfigInputModel inputModel)
        {
            // Check if config already exists for OrgId
            var existingConfig = await _context.OrganizationConfigurations
                .FirstOrDefaultAsync(c => c.OrgId == inputModel.OrgId);

            var config = existingConfig ?? new OrganizationConfiguration
            {
                // Set defaults for new row
                OrgId = inputModel.OrgId,
                EmailHost = "smtp.gmail.com",
                EmailPort = 587,
                EnableSSL = true,
                EmailTimeOut = 20000,
                SessionExpiryInMinutes = 60,
                ShortExpiryInMinutes = 1440,
                LongExpiryInMinutes = 525600,
                MaxRetryCount = 5,
                CreatedDate = DateTime.UtcNow
            };

            config.SetDataProtectionProvider(_dataProtectionProvider);

            // Update only provided fields
            // if (!string.IsNullOrEmpty(inputModel.Secret))
            //     config.Secret = inputModel.Secret; // Encrypted by setter

            if (!string.IsNullOrEmpty(inputModel.EmailHost))
                config.EmailHost = inputModel.EmailHost;

            if (inputModel.EmailPort.HasValue)
                config.EmailPort = inputModel.EmailPort.Value;

            if (inputModel.EnableSSL.HasValue)
                config.EnableSSL = inputModel.EnableSSL.Value;

            if (!string.IsNullOrEmpty(inputModel.EmailDefaultId))
                config.EmailDefaultId = inputModel.EmailDefaultId;

            if (!string.IsNullOrEmpty(inputModel.EmailPassword))
                config.EmailPassword = inputModel.EmailPassword; // Encrypted by setter

            if (inputModel.EmailTimeOut.HasValue)
                config.EmailTimeOut = inputModel.EmailTimeOut.Value;

            if (!string.IsNullOrEmpty(inputModel.CloudinaryCloudName))
                config.CloudinaryCloudName = inputModel.CloudinaryCloudName;

            if (!string.IsNullOrEmpty(inputModel.CloudinaryApiKey))
                config.CloudinaryApiKey = inputModel.CloudinaryApiKey;

            if (!string.IsNullOrEmpty(inputModel.CloudinaryApiSecret))
                config.CloudinaryApiSecret = inputModel.CloudinaryApiSecret; // Encrypted by setter

            if (inputModel.SessionExpiryInMinutes.HasValue)
                config.SessionExpiryInMinutes = inputModel.SessionExpiryInMinutes.Value;

            if (inputModel.ShortExpiryInMinutes.HasValue)
                config.ShortExpiryInMinutes = inputModel.ShortExpiryInMinutes.Value;

            if (inputModel.LongExpiryInMinutes.HasValue)
                config.LongExpiryInMinutes = inputModel.LongExpiryInMinutes.Value;

            if (inputModel.MaxRetryCount.HasValue)
                config.MaxRetryCount = inputModel.MaxRetryCount.Value;

            // Update timestamp
            config.UpdatedDate = DateTime.UtcNow;

            if (existingConfig == null)
            {
                _context.OrganizationConfigurations.Add(config);
            }

            await _context.SaveChangesAsync();

            // Update cache
            string cacheKey = $"OrgConfig_{inputModel.OrgId}";
            _cache.Set(cacheKey, config, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(15)
            });
        }

    }
}