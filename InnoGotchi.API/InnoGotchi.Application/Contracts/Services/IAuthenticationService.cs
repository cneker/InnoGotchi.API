using InnoGotchi.Application.DataTransferObjects.User;

namespace InnoGotchi.Application.Contracts.Services
{
    public interface IAuthenticationService
    {
        string CreatePasswordHash(string password);
        Task<bool> ValidateUserAsync(UserForAuthenticationDto userForAuth);
        string CreateToken();
    }
}
