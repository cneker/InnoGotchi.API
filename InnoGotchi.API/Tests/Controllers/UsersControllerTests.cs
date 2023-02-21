using InnoGotchi.API.Controllers;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly IFixture _fixture;
        private readonly ILogger<UsersController> _loggerMock;

        public UsersControllerTests()
        {
            _fixture = new Fixture();
            _loggerMock = Mock.Of<ILogger<UsersController>>();
        }

        [Fact]
        public async Task GetUsers_ReturnsOkResultAndListOfUserInfoDto()
        {
            //Arrange
            var usersInfo = _fixture.CreateMany<UserInfoDto>(5);
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.GetUsersInfoAsync())
                .Returns(Task.FromResult(usersInfo));

            var controller = new UsersController(userServiceMock.Object, _loggerMock);

            //Act
            var result = await controller.GetUsers();

            //Assert
            var okResult = result.Should().BeOfType<OkObjectResult>();
            (okResult.Which.Value as IEnumerable<UserInfoDto>).Should().NotBeNull()
                .And.AllBeOfType<UserInfoDto>()
                .And.HaveCount(5);
        }

        [Fact]
        public async Task GetUser_ReturnsOkResultAndUserInfoDto()
        {
            //Arrange
            var id = _fixture.Create<Guid>();
            var userInfo = _fixture.Build<UserInfoDto>()
                .With(u => u.Id, id)
                .Create();
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.GetUserInfoByIdAsync(id))
                .Returns(Task.FromResult(userInfo));

            var controller = new UsersController(userServiceMock.Object, _loggerMock);

            //Act
            var result = await controller.GetUser(id);

            //Assert
            var okResult = result.Should().BeOfType<OkObjectResult>();
            (okResult.Which.Value as UserInfoDto).Should().NotBeNull()
                .And.Match<UserInfoDto>(u => u.Id == id);
        }

        [Fact]
        public async Task CreateUser_ReturnsCreatedAdRouteResultAndUserEmail()
        {
            //Arrange
            var userForReg = _fixture.Create<UserForRegistrationDto>();
            var userInfo = _fixture.Build<UserInfoDto>()
                .With(u => u.Email, userForReg.Email)
                .With(u => u.LastName, userForReg.LastName)
                .With(u => u.FirstName, userForReg.FirstName)
                .Create();
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.CreateUserAsync(userForReg))
                .Returns(Task.FromResult(userInfo));

            var controller = new UsersController(userServiceMock.Object, _loggerMock);

            //Act
            var result = await controller.CreateUser(userForReg);

            //Assert
            var createdAtRouteResult = result.Should().BeOfType<CreatedAtRouteResult>();
            (createdAtRouteResult.Which.Value as UserInfoDto).Should().NotBeNull()
                .And.BeEquivalentTo(userInfo);
        }

        [Fact]
        public async Task UpdateUserInfo_ReturnsNoContentResult()
        {
            //Arrange
            var id = _fixture.Create<Guid>();
            var userInfoForUpdate = _fixture.Create<UserInfoForUpdateDto>();
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.UpdateUserInfoAsync(id, userInfoForUpdate))
                .Returns(Task.CompletedTask);

            var controller = new UsersController(userServiceMock.Object, _loggerMock);

            //Act
            var result = await controller.UpdateUserInfo(id, userInfoForUpdate);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ChangeUserPassword_ReturnsNoContentResult()
        {
            //Arrange
            var id = _fixture.Create<Guid>();
            var passwordChanging = _fixture.Create<PasswordChangingDto>();
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.UpdatePasswordAsync(id, passwordChanging))
                .Returns(Task.CompletedTask);

            var controller = new UsersController(userServiceMock.Object, _loggerMock);

            //Act
            var result = await controller.ChangeUserPassword(id, passwordChanging);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContentResult()
        {
            //Arrange
            var id = _fixture.Create<Guid>();
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.DeleteUserById(id))
                .Returns(Task.CompletedTask);

            var controller = new UsersController(userServiceMock.Object, _loggerMock);

            //Act
            var result = await controller.DeleteUser(id);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateAvatar_ReturnsNoContentResult()
        {
            //Arrange
            var id = _fixture.Create<Guid>();
            var avatarChangingDto = _fixture.Create<AvatarChangingDto>();
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.UpdateAvatarAsync(id, avatarChangingDto))
                .Returns(Task.CompletedTask);

            var controller = new UsersController(userServiceMock.Object, _loggerMock);

            //Act
            var result = await controller.UpdateAvatar(id, avatarChangingDto);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
