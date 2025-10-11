namespace NGO.Model
{
    public class ResultModel
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string Error { get; set; }
        public string Details { get; set; }
    }

}
