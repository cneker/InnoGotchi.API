using Infrastructure.Services;
using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.User;
using InnoGotchi.Application.Exceptions;
using InnoGotchi.Domain.Entities;

namespace Tests.Services
{
    public class AuthenticationServiceTests
    {
        private readonly Fixture _fixture;
        public AuthenticationServiceTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void VerifyPasswordHash_ReturnsTrue_WhenPasswordAndItsHashAreValid()
        {
            //Arrange
            var passwodHash = "$2b$10$gj7WVHAVH28V5ZNYde0y0.nx7PY7/i1wvNZ91B6bhUnj48FWcEh/a";
            var password = "qwerty";
            var service = new AuthenticationService(null, null);

            //Act
            var result = service.VerifyPasswordHash(password, passwodHash);
            //Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("qwerty123", "$2b$10$gj7WVHAVH28V5ZNYde0y0.nx7PY7/i1wvNZ91B6bhUnj48FWcEh/a")]
        [InlineData("qwerty", "$2b$10$gj7WVHAVH28V5ZNYde0y0.nx7PY7/i1wvNZ91B7bhUnj48FWcEh/a")]
        public void VerifyPasswordHash_ReturnsFalse_WhenPasswordOrItsHashAreInvalid(string password, string passwodHash)
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
        public async Task SignInAsync_ReturnsJWT_WhenUserEmailAndPasswordAreValid()
        {
            //Arrange
            var token = _fixture.Create<string>();
            var userDto = _fixture.Build<UserForAuthenticationDto>()
                .With(u => u.Password, "qwerty")
                .Create();
            var user = _fixture.Build<User>()
                .With(u => u.PasswordHash, "$2b$10$gj7WVHAVH28V5ZNYde0y0.nx7PY7/i1wvNZ91B6bhUnj48FWcEh/a")
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();

            var repMock = new Mock<IRepositoryManager>();
            repMock.Setup(a => a.UserRepository.GetUserByEmailAsync(It.IsAny<string>(), false))
                .Returns(Task.FromResult(user));

            var tokenMock = new Mock<IGenerateTokenService>();
            tokenMock.Setup(a => a.GenerateToken(user))
                .Returns(token);

            var service = new AuthenticationService(repMock.Object, tokenMock.Object);

            //Act
            var result = await service.SignInAsync(userDto);

            //Assert
            token.Should().Be(result);
            tokenMock.Verify(a => a.GenerateToken(user), Times.Once);
        }

        [Fact]
        public async Task SignInAsync_ThrowingNotFoundException_WhenUserEmailIsInvalid()
        {
            //Arrange
            var userDto = _fixture.Build<UserForAuthenticationDto>()
                .With(u => u.Password, "qwerty")
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
        public async Task SignInAsync_ThrowingIncorrectRequestException_WhenUserPasswordIsInvalid()
        {
            //Arrange
            var userDto = _fixture.Build<UserForAuthenticationDto>()
                .With(u => u.Password, "qwerty123")
                .Create();
            var user = _fixture.Build<User>()
                .With(u => u.PasswordHash, "$2b$10$gj7WVHAVH28V5ZNYde0y0.nx7PY7/i1wvNZ91B6bhUnj48FWcEh/a")
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
