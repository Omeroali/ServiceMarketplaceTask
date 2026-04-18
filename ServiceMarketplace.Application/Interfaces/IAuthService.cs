namespace ServiceMarketplace.Application.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(string email, string password, string fullName);
    Task<string> LoginAsync(string email, string password);
    Task CheckUserLimit(string userId);

}