namespace OnConcert.BL.Models
{
    public static class EmptyServiceResponseBuilder
    {
        public static EmptyServiceResponse CreateSuccessResponse(string successMessage = "") =>
            new()
            {
                Message = successMessage
            };

        public static EmptyServiceResponse CreateErrorResponse(string errorMessage) =>
            new()
            {
                Success = false,
                Message = errorMessage
            };
    }
}