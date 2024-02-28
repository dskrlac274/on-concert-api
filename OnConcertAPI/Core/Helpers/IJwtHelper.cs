namespace OnConcert.Core.Helpers
{
    public interface IJwtHelper
    {
        public string CreateJwt(int userId, string role);
    }
}