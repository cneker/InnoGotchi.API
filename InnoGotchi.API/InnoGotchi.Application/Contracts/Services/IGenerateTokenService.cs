using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.Contracts.Services
{
    public interface IGenerateTokenService
    {
        string GenerateToken(User user);
    }
}
