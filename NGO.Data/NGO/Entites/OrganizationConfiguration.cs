using Microsoft.AspNetCore.DataProtection;

namespace NGO.Data
{
    public partial class OrganizationConfiguration
    {
        private IDataProtector? _protector;

        public OrganizationConfiguration()
        {
            // Parameterless constructor for EF Core
        }

        // Method to set the protector after instantiation
        public void SetDataProtectionProvider(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("OrganizationConfiguration");
        }
        public int Id { get; set; }
        public int OrgId { get; set; }

        //private string _secret = string.Empty;
        private string _emailPassword = string.Empty;
        private string _cloudinaryApiSecret = string.Empty;

        // public string Secret
        // {
        //     get => _protector != null ? _protector.Unprotect(_secret) : _secret;
        //     set => _secret = _protector != null ? _protector.Protect(value) : value;
        // }

        public string EmailPassword
        {
            get => _protector != null ? _protector.Unprotect(_emailPassword) : _emailPassword;
            set => _emailPassword = _protector != null ? _protector.Protect(value) : value;
        }

        public string CloudinaryApiSecret
        {
            get => _protector != null ? _protector.Unprotect(_cloudinaryApiSecret) : _cloudinaryApiSecret;
            set => _cloudinaryApiSecret = _protector != null ? _protector.Protect(value) : value;
        }

        public int SessionExpiryInMinutes { get; set; }
        public int ShortExpiryInMinutes { get; set; }
        public int LongExpiryInMinutes { get; set; }
        public int MaxRetryCount { get; set; }
        public string EmailHost { get; set; } = string.Empty;
        public int EmailPort { get; set; }
        public bool EnableSSL { get; set; }
        public string EmailDefaultId { get; set; } = string.Empty;
        public int EmailTimeOut { get; set; }
        public string CloudinaryCloudName { get; set; } = string.Empty;
        public string CloudinaryApiKey { get; set; } = string.Empty;
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }

        public virtual Organization Organization { get; set; } = null!;
    }
}