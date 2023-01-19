using BCrypt.Net;
using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.User;
using InnoGotchi.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IConfiguration _configuration;
        private User _user;
        public AuthenticationService(IRepositoryManager repositoryManager,
            IConfiguration configuration)
        {
            _repositoryManager = repositoryManager;
            _configuration = configuration;
        }
        public string CreatePasswordHash(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password, SaltRevision.Revision2B);

        public async Task<bool> ValidateUserAsync(UserForAuthenticationDto userForAuth)
        {
            _user = await _repositoryManager.UserRepository
                .GetUserByEmailAsync(userForAuth.Email, false);

            if (_user == null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(userForAuth.Password, _user.PasswordHash))
                return false;

            return true;
        }

        public string CreateToken()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key =
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"));
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private List<Claim> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _user.Id.ToString())
            };

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials,
            List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var tokenOptions = new JwtSecurityToken
                (
                issuer: jwtSettings.GetSection("validIssuer").Value,
                audience: jwtSettings.GetSection("validAudience").Value,
                claims: claims,
                expires:
                    DateTime.Now.AddDays(int.Parse(jwtSettings.GetSection("expires").Value)),
                signingCredentials: signingCredentials
                );

            return tokenOptions;
        }
    }
}
