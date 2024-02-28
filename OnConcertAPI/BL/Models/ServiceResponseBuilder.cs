namespace OnConcert.BL.Models
{
    public static class ServiceResponseBuilder
    {
        public static ServiceResponse<T> CreateSuccessResponse<T>(T data, string successMessage = "") where T : class =>
            new()
            {
                Data = data,
                Message = successMessage
            };

        public static ServiceResponse<T> CreateErrorResponse<T>(string errorMessage) where T : class =>
            new()
            {
                Success = false,
                Message = errorMessage
            };
    }
}