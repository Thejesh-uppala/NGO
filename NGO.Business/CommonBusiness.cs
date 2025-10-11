using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using NGO.Repository;
using NGO.Repository.Contracts;

namespace NGO.Business
{
    public class CommonBusiness
    {
        private readonly IUserRepository _userRepository;

        private readonly IOrganizationConfigService _configService;

        public CommonBusiness(IUserRepository userRepository, IOrganizationConfigService configService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        }

        public string FormatEventTime(string eventTimeUtc)
        {
            if (string.IsNullOrWhiteSpace(eventTimeUtc))
                return "Invalid time format";

            var times = eventTimeUtc.Split('-');
            if (times.Length != 2)
                return "Invalid time format";

            // Convert the times to 12-hour format
            var startTime = ConvertTo12HourFormat(times[0].Trim());
            var endTime = ConvertTo12HourFormat(times[1].Trim());

            // Return the formatted times as "Start Time - End Time"
            return $"{startTime} - {endTime}";
        }

        public string ConvertTo12HourFormat(string time)
        {
            if (string.IsNullOrWhiteSpace(time))
                return "Invalid time";

            // Parse the time string (assumed to be in "HH:mm" format)
            if (!TimeSpan.TryParse(time, out var parsedTime))
                return "Invalid time";

            // Format the time to 12-hour format
            var date = DateTime.Today.Add(parsedTime);
            return date.ToString("hh:mm tt"); // "hh:mm AM/PM" format
        }



        public async Task<string> GetEmailTemplateFromCloudinaryAsync(string category, string templateFileName, int orgId)
        {

            var orgConfig = await _configService.GetConfigByOrgId(orgId);

            if (string.IsNullOrWhiteSpace(orgConfig.CloudinaryCloudName) ||
                string.IsNullOrWhiteSpace(orgConfig.CloudinaryApiKey) ||
                string.IsNullOrWhiteSpace(orgConfig.CloudinaryApiSecret))
            {
                throw new InvalidOperationException($"Cloudinary credentials are missing or empty for OrgId {orgId}.");
            }

            var cloudinary = new Cloudinary(new Account(
                orgConfig.CloudinaryCloudName,
                orgConfig.CloudinaryApiKey,
                orgConfig.CloudinaryApiSecret));

            try
            {
                string fullPath = $"templates/{category}/{templateFileName}";

                var getResourceParams = new GetResourceParams(fullPath)
                {
                    ResourceType = ResourceType.Raw
                };

                var resource = await cloudinary.GetResourceAsync(getResourceParams);

                if (resource == null)
                {
                    Console.WriteLine($"Template '{templateFileName}' not found in Cloudinary.");
                }

                string templateUrl = resource.SecureUrl;
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(templateUrl);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public async Task<string> UploadToCloudinaryAsync(IFormFile file, int orgId)
        {
            var orgConfig = await _configService.GetConfigByOrgId(orgId);

            if (string.IsNullOrWhiteSpace(orgConfig.CloudinaryCloudName) ||
                string.IsNullOrWhiteSpace(orgConfig.CloudinaryApiKey) ||
                string.IsNullOrWhiteSpace(orgConfig.CloudinaryApiSecret))
            {
                throw new InvalidOperationException($"Cloudinary credentials are missing or empty for OrgId {orgId}.");
            }

            var cloudinary = new Cloudinary(new Account(
                orgConfig.CloudinaryCloudName,
                orgConfig.CloudinaryApiKey,
                orgConfig.CloudinaryApiSecret));

            // Open the file stream
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Unsigned = true,
                UploadPreset = "freetest"
            };
            var uploadResult = cloudinary.Upload(uploadParams);

            return uploadResult?.SecureUri?.ToString();
        }

    }
}