using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.Exceptions;
using InnoGotchi.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class GenerateTokenService : IGenerateTokenService
    {
        private readonly IConfiguration _configuration;
        public GenerateTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(User user)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Environment.GetEnvironmentVariable("SECRET");
            if (key == null)
                throw new MissingEnvironmentVariableException("SECRET");
            var keyInBytes =
                Encoding.UTF8.GetBytes(key);
            var secret = new SymmetricSecurityKey(keyInBytes);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private List<Claim> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials,
            List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var issuer = jwtSettings.GetRequiredSection("validIssuer").Value;
            var audience = jwtSettings.GetRequiredSection("validAudience").Value;
            var expires = jwtSettings.GetRequiredSection("expires").Value;

            var tokenOptions = new JwtSecurityToken
                (
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires:
                    DateTime.Now.AddDays(int.Parse(expires)),
                signingCredentials: signingCredentials
                );

            return tokenOptions;
        }
    }
}
