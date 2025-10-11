namespace NGO.Model
{
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public int StatusCode { get; private set; }
        public string Message { get; private set; }
        public string Error { get; private set; }

        // Factory method for successful result
        public static Result Success(int statusCode, string message = null)
        {
            return new Result
            {
                IsSuccess = true,
                StatusCode = statusCode,
                Message = message
            };
        }

        // Factory method for failed result
        public static Result Failure(int statusCode, string error)
        {
            return new Result
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Error = error
            };
        }
    }

}
