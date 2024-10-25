namespace NGO.Common
{
    public class EmailSettings
    {
        public string? EmailHost { get; set; }
        public int EmailPort { get; set; }
        public bool EnableSSL { get; set; }
        public string? EmailDefaultId { get; set; }
        public string? EmailPassword { get; set; }
        public int EmailTimeOut { get; set; }
        public string? ApiKey { get; set; }
    }
}
