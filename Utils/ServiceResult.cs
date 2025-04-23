namespace BookingService.Utils
{
    public class ServiceResult
    {
        public bool Success { get; private set; }
        public string? ErrorMessage { get; private set; }
        public int StatusCode { get; private set; }

        protected ServiceResult(bool success, string? message = null, int statusCode = 0)
        {
            Success = success;
            ErrorMessage = message;
            StatusCode = statusCode;
        }

        public static ServiceResult SuccessResult(int statusCode = 200)
        {
            return new ServiceResult(true, null, statusCode);
        }

        public static ServiceResult Failure(string errorMessage, int statusCode = 400)
        {
            return new ServiceResult(false, errorMessage, statusCode);
        }
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; private set; }

        private ServiceResult(bool success, T? data = default, string? errorMessage = null, int statusCode = 0) : base(success, errorMessage, statusCode)
        {
            Data = data;
        }

        public static ServiceResult<T> SuccessResult(T data, int statusCode = 200)
        {
            return new ServiceResult<T>(true, data, null, statusCode);
        }

        public static new ServiceResult<T> Failure(string errorMessage, int statusCode = 400)
        {
            return new ServiceResult<T>(false, default, errorMessage, statusCode);
        }
    }
}
