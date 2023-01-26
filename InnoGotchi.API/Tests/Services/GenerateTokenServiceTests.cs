using Infrastructure.Services;
using InnoGotchi.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Tests.Services
{
    public class GenerateTokenServiceTests
    {
        private readonly IFixture _fixture;
        public GenerateTokenServiceTests()
        {
            _fixture = new Fixture();
        }
        
        [Fact]
        public void GenerateToken_ReturnsJWT()
        {
            //Arrange
            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(x => x.GetSection("validIssuer").Value).Returns(_fixture.Create<string>());
            mockSection.Setup(x => x.GetSection("validAudience").Value).Returns(_fixture.Create<string>());
            mockSection.Setup(x => x.GetSection("expires").Value).Returns("1");

            var confMock = new Mock<IConfiguration>();
            confMock.Setup(a => a.GetSection("JwtSettings"))
                .Returns(mockSection.Object);

            var user = _fixture.Build<User>()
                .Without(u => u.FriendsFarms)
                .Without(u => u.UserFarm)
                .Create();
            var tokenService = new GenerateTokenService(confMock.Object);

            //Act
            var result = tokenService.GenerateToken(user);

            //Assert
            result.Should().BeOfType<string>();
            confMock.Verify(a => a.GetSection("JwtSettings"), Times.Once);
            mockSection.Verify(a => a.GetSection("validIssuer").Value, Times.Once);
            mockSection.Verify(a => a.GetSection("validAudience").Value, Times.Once);
            mockSection.Verify(a => a.GetSection("expires").Value, Times.Once);
        }
    }
}
