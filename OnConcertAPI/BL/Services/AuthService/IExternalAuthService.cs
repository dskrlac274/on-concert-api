using OnConcert.BL.Models;

namespace OnConcert.BL.Services.AuthService
{
    public interface IExternalAuthService
    {
        Task<ServiceResponse<object>> Authenticate(string token);
    }
}
