namespace NGO.Model
{
    public class OrganizationConfigInputModel
    {
        public int OrgId { get; set; }
        // public string? Secret { get; set; } // Null if not provided
        public string? EmailHost { get; set; } // Null if not provided
        public int? EmailPort { get; set; } // Null if not provided
        public bool? EnableSSL { get; set; } // Null if not provided
        public string? EmailDefaultId { get; set; } // Null if not provided
        public string? EmailPassword { get; set; } // Null if not provided
        public int? EmailTimeOut { get; set; } // Null if not provided
        public string? CloudinaryCloudName { get; set; } // Null if not provided
        public string? CloudinaryApiKey { get; set; } // Null if not provided
        public string? CloudinaryApiSecret { get; set; } // Null if not provided
        public int? SessionExpiryInMinutes { get; set; } // Null if not provided
        public int? ShortExpiryInMinutes { get; set; } // Null if not provided
        public int? LongExpiryInMinutes { get; set; } // Null if not provided
        public int? MaxRetryCount { get; set; } // Null if not provided
    }
}