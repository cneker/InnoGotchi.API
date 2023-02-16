using BCrypt.Net;
using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects;
using InnoGotchi.Application.DataTransferObjects.User;
using InnoGotchi.Application.Exceptions;

namespace Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IGenerateTokenService _tokenService;

        public AuthenticationService(IRepositoryManager repositoryManager,
            IGenerateTokenService tokenService)
        {
            _repositoryManager = repositoryManager;
            _tokenService = tokenService;
        }
        public string CreatePasswordHash(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password, SaltRevision.Revision2B);

        public async Task<AccessTokenDto> SignInAsync(UserForAuthenticationDto userForAuth)
        {
            var user = await _repositoryManager.UserRepository
                .GetUserByEmailAsync(userForAuth.Email, false);

            if (user == null)
                throw new NotFoundException("User not found");

            if (!VerifyPasswordHash(userForAuth.Password, user.PasswordHash))
                throw new IncorrectRequestException("Wrong password");

            var jwt = _tokenService.GenerateToken(user);

            return new AccessTokenDto { AccessToken= jwt, UserId = user.Id };
        }

        public bool VerifyPasswordHash(string password, string passwordHash) =>
            BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
