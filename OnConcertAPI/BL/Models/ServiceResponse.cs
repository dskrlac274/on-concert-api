namespace OnConcert.BL.Models
{
    public class ServiceResponse<T> : EmptyServiceResponse where T : class
    {
        public T? Data { get; set; }
    }
}