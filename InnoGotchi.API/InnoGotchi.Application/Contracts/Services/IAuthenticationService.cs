using InnoGotchi.Application.DataTransferObjects.User;

namespace InnoGotchi.Application.Contracts.Services
{
    public interface IAuthenticationService
    {
        string CreatePasswordHash(string password);
        Task<string> SignInAsync(UserForAuthenticationDto userForAuth);
        bool VerifyPasswordHash(string password, string passwordHash);
    }
}
