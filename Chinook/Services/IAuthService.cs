namespace Chinook.Services
{
    public interface IAuthService
    {
        Task<string> GetUserIdAsync();
    }
}
