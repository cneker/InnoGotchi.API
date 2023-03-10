using Infrastructure.Services;
using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects;
using InnoGotchi.Application.DataTransferObjects.User;
using InnoGotchi.Application.Exceptions;
using InnoGotchi.Domain.Entities;

namespace Tests.Services
{
    public class AuthenticationServiceTests
    {
        private const string ValidPasswordHash = "$2b$10$gj7WVHAVH28V5ZNYde0y0.nx7PY7/i1wvNZ91B6bhUnj48FWcEh/a";
        private const string ValidPassword = "qwerty";
        private readonly Fixture _fixture;
        public AuthenticationServiceTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void VerifyPasswordHash_WhenPasswordAndItsHashAreValid_ReturnsTrue()
        {
            //Arrange
            var service = new AuthenticationService(null, null);

            //Act
            var result = service.VerifyPasswordHash(ValidPassword, ValidPasswordHash);
            //Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("qwerty123", ValidPasswordHash)]
        [InlineData(ValidPassword, "$2b$10$gj7WVHAVH28V5ZNYde0y0.nx7PY7/i1wvNZ91B7bhUnj48FWcEh/a")]
        public void VerifyPasswordHash_WhenPasswordOrItsHashAreInvalid_ReturnsFalse(string password, string passwodHash)
        {
            //Arrange
            var service = new AuthenticationService(null, null);

            //Act
            var result = service.VerifyPasswordHash(password, passwodHash);
            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void CreatePasswordHash_ReturnsPasswordHash()
        {
            //Arrange
            var password = _fixture.Create<string>();
            var service = new AuthenticationService(null, null);

            //Act
            var hash = service.CreatePasswordHash(password);

            //Assert
            service.VerifyPasswordHash(password, hash).Should().BeTrue();
        }

        [Fact]
        public async Task SignInAsync_WhenUserEmailAndPasswordAreValid_ReturnsJWT()
        {
            //Arrange
            var userDto = _fixture.Build<UserForAuthenticationDto>()
                .With(u => u.Password, ValidPassword)
                .Create();
            var user = _fixture.Build<User>()
                .With(u => u.PasswordHash, ValidPasswordHash)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            var token = _fixture.Build<AccessTokenDto>()
                .With(t => t.UserId, user.Id)
                .Create();

            var repMock = new Mock<IRepositoryManager>();
            repMock.Setup(a => a.UserRepository.GetUserByEmailAsync(It.IsAny<string>(), false))
                .Returns(Task.FromResult(user));

            var tokenMock = new Mock<IGenerateTokenService>();
            tokenMock.Setup(a => a.GenerateToken(user))
                .Returns(token.AccessToken);

            var service = new AuthenticationService(repMock.Object, tokenMock.Object);

            //Act
            var result = await service.SignInAsync(userDto);

            //Assert
            token.Should().BeEquivalentTo(result);
            tokenMock.Verify(a => a.GenerateToken(user), Times.Once);
        }

        [Fact]
        public async Task SignInAsync_WhenUserEmailIsInvalid_ThrowingNotFoundException()
        {
            //Arrange
            var userDto = _fixture.Build<UserForAuthenticationDto>()
                .With(u => u.Password, ValidPassword)
                .Create();

            var repMock = new Mock<IRepositoryManager>();
            repMock.Setup(a => a.UserRepository.GetUserByEmailAsync(It.IsAny<string>(), false))
                .Returns(Task.FromResult<User>(null));

            var service = new AuthenticationService(repMock.Object, null);

            //Act
            Func<Task> result = async () => await service.SignInAsync(userDto);

            //Assert
            await result.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        }

        [Fact]
        public async Task SignInAsync_WhenUserPasswordIsInvalid_ThrowingIncorrectRequestException()
        {
            //Arrange
            var userDto = _fixture.Build<UserForAuthenticationDto>()
                .With(u => u.Password, "qwerty123")
                .Create();
            var user = _fixture.Build<User>()
                .With(u => u.PasswordHash, ValidPasswordHash)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();

            var repMock = new Mock<IRepositoryManager>();
            repMock.Setup(a => a.UserRepository.GetUserByEmailAsync(It.IsAny<string>(), false))
                .Returns(Task.FromResult(user));

            var service = new AuthenticationService(repMock.Object, null);

            //Act
            Func<Task> result = async () => await service.SignInAsync(userDto);

            //Assert
            await result.Should().ThrowAsync<IncorrectRequestException>().WithMessage("Wrong password");
        }
    }
}
