using InnoGotchi.Application.DataTransferObjects;
using InnoGotchi.Application.DataTransferObjects.User;

namespace InnoGotchi.Application.Contracts.Services
{
    public interface IAuthenticationService
    {
        string CreatePasswordHash(string password);
        Task<AccessTokenDto> SignInAsync(UserForAuthenticationDto userForAuth);
        bool VerifyPasswordHash(string password, string passwordHash);
    }
}
