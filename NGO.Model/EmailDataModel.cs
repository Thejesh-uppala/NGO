namespace NGO.Model
{
    public class EmailDataModel
    {
        public List<string> To { get; set; } = new List<string>();
        public List<string> Bcc { get; set; } = new List<string>();
        public List<string> CC { get; set; } = new List<string>();
        public string From { get; set; }
        public string Subject { get; set; }
        public string Data { get; set; }
        public bool IsSuccess { get; set; }
    }
}
