﻿using BCrypt.Net;
using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.User;

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

        public async Task<string> SignInAsync(UserForAuthenticationDto userForAuth)
        {
            //сюда бы валидатор добавить
            var user = await _repositoryManager.UserRepository
                .GetUserByEmailAsync(userForAuth.Email, false);

            if (user == null)
                throw new Exception("not found");

            if (!VerifyPasswordHash(userForAuth.Password, user.PasswordHash))
                throw new Exception("wrong password");

            return _tokenService.GenerateToken(user);
        }

        public bool VerifyPasswordHash(string password, string passwordHash) =>
            BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
