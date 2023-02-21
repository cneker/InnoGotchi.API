using AutoMapper;
using Infrastructure.Services;
using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.User;
using InnoGotchi.Application.Exceptions;
using InnoGotchi.Domain.Entities;

namespace Tests.Services
{
    public class UserServiceTests
    {
        private readonly Fixture _fixture;
        public UserServiceTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsUserInfoDto_WhenPassedEmailIsUniq()
        {
            //Arrange
            var userForReg = _fixture.Create<UserForRegistrationDto>();
            var user = _fixture.Build<User>()
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            var userInfo = _fixture.Build<UserInfoDto>()
                .With(u => u.Id, user.Id)
                .Create();

            var repositoryMock = new Mock<IRepositoryManager>();
            repositoryMock.Setup(r => r.UserRepository.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(Task.FromResult<User>(null));
            repositoryMock.Setup(r => r.UserRepository.CreateUser(user))
                .Returns(Task.CompletedTask);
            repositoryMock.Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);

            var authMock = new Mock<IAuthenticationService>();
            authMock.Setup(a => a.CreatePasswordHash(It.IsAny<string>()))
                .Returns(_fixture.Create<string>());

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<User>(It.IsAny<UserForRegistrationDto>()))
                .Returns(user);
            mapperMock.Setup(m => m.Map<UserInfoDto>(user))
                .Returns(userInfo);

            var service = new UserService(repositoryMock.Object, authMock.Object, mapperMock.Object, null);
            //Act
            var result = await service.CreateUserAsync(userForReg);

            //Assert
            repositoryMock.Verify(r => r.UserRepository.CreateUser(It.IsAny<User>()), Times.Once);
            repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
            mapperMock.Verify(m => m.Map<User>(It.IsAny<UserForRegistrationDto>()), Times.Once);
            authMock.Verify(a => a.CreatePasswordHash(It.IsAny<string>()), Times.Once);
            result.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task CreateUserAsync_ThrowingAlreadyExistsException_WhenPassedEmailIsAlreadyRegistered()
        {
            //Arrange
            var userForReg = _fixture.Create<UserForRegistrationDto>();
            var user = _fixture.Build<User>()
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();

            var repositoryMock = new Mock<IRepositoryManager>();
            repositoryMock.Setup(r => r.UserRepository.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(user));

            var service = new UserService(repositoryMock.Object, null, null, null);

            //Act
            Func<Task> result = async () => await service.CreateUserAsync(userForReg);

            //Assert
            await result.Should().ThrowAsync<AlreadyExistsException>().WithMessage("The email has already registered");
        }

        [Fact]
        public async Task GetUsersInfoAsync_ReturnsUsersList()
        {
            //Arrange
            var users = _fixture.Build<User>()
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .CreateMany();
            var userInfoDtos = _fixture.CreateMany<UserInfoDto>();

            var repositoryMock = new Mock<IRepositoryManager>();
            repositoryMock.Setup(r => r.UserRepository.GetUsersAsync(It.IsAny<bool>()))
                .Returns(Task.FromResult(users));

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<IEnumerable<UserInfoDto>>(users))
                .Returns(userInfoDtos);

            var service = new UserService(repositoryMock.Object, null, mapperMock.Object, null);

            //Act
            var result = await service.GetUsersInfoAsync();

            //Assert
            result.Should().NotBeNull()
                .And.AllBeOfType<UserInfoDto>();
        }

        [Fact]
        public async Task DeleteUserById_DeletedUser_WhenPassedIdIsValid()
        {
            //Arrange
            var id = _fixture.Create<Guid>();
            var farm = _fixture.Build<Farm>()
                .Without(f => f.Pets)
                .Without(f => f.User)
                .Without(f => f.Collaborators)
                .Create();
            var user = _fixture.Build<User>()
                .With(u => u.Id, id)
                .With(u => u.UserFarm, farm)
                .Without(u => u.FriendsFarms)
                .Create();

            var repositoryMock = new Mock<IRepositoryManager>();
            repositoryMock.Setup(r => r.UserRepository.GetUserByIdAsync(id, It.IsAny<bool>()))
                .Returns(Task.FromResult(user));
            repositoryMock.Setup(r => r.UserRepository.DeleteUser(user));
            repositoryMock.Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);
            repositoryMock.Setup(r => r.FarmRepository.DeleteFarm(user.UserFarm));

            var service = new UserService(repositoryMock.Object, null, null, null);

            //Act
            await service.DeleteUserById(id);

            //Assert
            repositoryMock.Verify(r => r.FarmRepository.DeleteFarm(user.UserFarm), Times.Once);
            repositoryMock.Verify(r => r.UserRepository.DeleteUser(user), Times.Once);
            repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteUserById_ThrowingNotFoundException_WhenPassedIdIsInvalid()
        {
            //Arrange
            var id = _fixture.Create<Guid>();

            var repositoryMock = new Mock<IRepositoryManager>();
            repositoryMock.Setup(r => r.UserRepository.GetUserByIdAsync(id, It.IsAny<bool>()))
                .Returns(Task.FromResult<User>(null));

            var service = new UserService(repositoryMock.Object, null, null, null);

            //Act
            Func<Task> result = async () => await service.DeleteUserById(id);

            //Assert
            await result.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        }

        [Fact]
        public async Task GetUserInfoByIdAsync_ReturnsUserInfoDto_WhenPassedIdIsValid()
        {
            //Arrange
            var id = _fixture.Create<Guid>();
            var user = _fixture.Build<User>()
                .With(u => u.Id, id)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            var userInfoDto = _fixture.Create<UserInfoDto>();
            var repositoryMock = new Mock<IRepositoryManager>();
            repositoryMock.Setup(r => r.UserRepository.GetUserByIdAsync(id, It.IsAny<bool>()))
                .Returns(Task.FromResult(user));
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<UserInfoDto>(user))
                .Returns(userInfoDto);

            var service = new UserService(repositoryMock.Object, null, mapperMock.Object, null);

            //Act
            var result = await service.GetUserInfoByIdAsync(user.Id);

            //Assert
            mapperMock.Verify(m => m.Map<UserInfoDto>(user), Times.Once);
            result.Should().Be(userInfoDto);
        }

        [Fact]
        public async Task GetUserInfoByIdAsync_ThrowingNotFoundException_WhenPassedIdIsInvalid()
        {
            //Arrange
            var id = _fixture.Create<Guid>();
            var repositoryMock = new Mock<IRepositoryManager>();
            repositoryMock.Setup(r => r.UserRepository.GetUserByIdAsync(id, It.IsAny<bool>()))
                .Returns(Task.FromResult<User>(null));

            var service = new UserService(repositoryMock.Object, null, null, null);

            //Act
            Func<Task> result = async () => await service.GetUserInfoByIdAsync(id);

            //Assert
            await result.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        }

        [Fact]
        public async Task UpdatePasswordAsync_UpdatedPasswordHash_WhenPassedIdIsValid()
        {
            //Arrange
            var id = _fixture.Create<Guid>();
            var user = _fixture.Build<User>()
                .With(u => u.Id, id)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            var passwordChangingDto = _fixture.Create<PasswordChangingDto>();
            var repositorMock = new Mock<IRepositoryManager>();
            repositorMock.Setup(r => r.UserRepository.GetUserByIdAsync(id, true))
                .Returns(Task.FromResult(user));
            repositorMock.Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);
            var authMock = new Mock<IAuthenticationService>();
            authMock.Setup(a => a.CreatePasswordHash(passwordChangingDto.NewPassword))
                .Returns(_fixture.Create<string>());

            var service = new UserService(repositorMock.Object, authMock.Object, null, null);

            //Act
            await service.UpdatePasswordAsync(id, passwordChangingDto);

            //Assert
            authMock.Verify(a => a.CreatePasswordHash(passwordChangingDto.NewPassword), Times.Once);
            repositorMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdatePasswordAsync_ThrowingNotFoundExceptions_WhenPassedIdIsInvalid()
        {
            //Arrange
            var id = _fixture.Create<Guid>();
            var passwordChangingDto = _fixture.Create<PasswordChangingDto>();
            var repositorMock = new Mock<IRepositoryManager>();
            repositorMock.Setup(r => r.UserRepository.GetUserByIdAsync(id, true))
                .Returns(Task.FromResult<User>(null));

            var service = new UserService(repositorMock.Object, null, null, null);

            //Act
            Func<Task> result = async () => await service.UpdatePasswordAsync(id, passwordChangingDto);

            //Assert
            await result.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        }

        [Fact]
        public async Task UpdateUserInfoAsync_UpdatedUser_WhenPassedIdIsValid()
        {
            //Arrange
            var id = _fixture.Create<Guid>();
            var user = _fixture.Build<User>()
                .With(u => u.Id, id)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            var userInfoForUpdateDto = _fixture.Create<UserInfoForUpdateDto>();
            var repositorMock = new Mock<IRepositoryManager>();
            repositorMock.Setup(r => r.UserRepository.GetUserByIdAsync(id, true))
                .Returns(Task.FromResult(user));
            repositorMock.Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map(userInfoForUpdateDto, user))
                .Returns(user);

            var service = new UserService(repositorMock.Object, null, mapperMock.Object, null);

            //Act
            await service.UpdateUserInfoAsync(id, userInfoForUpdateDto);

            //Assert
            mapperMock.Verify(a => a.Map(userInfoForUpdateDto, user), Times.Once);
            repositorMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateUserInfoAsync_ThrowingNotFoundException_WhenPassedIdIsInvalid()
        {
            //Arrange
            var id = _fixture.Create<Guid>();
            var userInfoForUpdateDto = _fixture.Create<UserInfoForUpdateDto>();
            var repositorMock = new Mock<IRepositoryManager>();
            repositorMock.Setup(r => r.UserRepository.GetUserByIdAsync(id, true))
                .Returns(Task.FromResult<User>(null));

            var service = new UserService(repositorMock.Object, null, null, null);

            //Act
            Func<Task> result = async () => await service.UpdateUserInfoAsync(id, userInfoForUpdateDto);

            //Assert
            await result.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        }

        [Fact]
        public async Task GetUserInfoForLayoutByIdAsync_ReturnsUserInfoForLayoutDto_WhenPassedIdIsValid()
        {
            //Arrange
            var id = _fixture.Create<Guid>();
            var user = _fixture.Build<User>()
                .With(u => u.Id, id)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            var userInfoForLayout = _fixture.Create<UserInfoForLayoutDto>();
            var repositoryMock = new Mock<IRepositoryManager>();
            repositoryMock.Setup(r => r.UserRepository.GetUserByIdAsync(id, false))
                .Returns(Task.FromResult(user));
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<UserInfoForLayoutDto>(user))
                .Returns(userInfoForLayout);

            var service = new UserService(repositoryMock.Object, null, mapperMock.Object, null);

            //Act
            var result = await service.GetUserInfoForLayoutByIdAsync(id);

            //Assert
            mapperMock.Verify(m => m.Map<UserInfoForLayoutDto>(user), Times.Once());
            result.Should().BeEquivalentTo(userInfoForLayout);
        }

        [Fact]
        public async Task GetUserInfoForLayoutByIdAsync_ThrowingNotFoundException_WhenPassedIdIsInvalid()
        {
            //Arrange
            var id = _fixture.Create<Guid>();
            var repositoryMock = new Mock<IRepositoryManager>();
            repositoryMock.Setup(r => r.UserRepository.GetUserByIdAsync(id, false))
                .Returns(Task.FromResult<User>(null));

            var service = new UserService(repositoryMock.Object, null, null, null);

            //Act
            Func<Task> result = async () => await service.GetUserInfoForLayoutByIdAsync(id);

            //Assert
            await result.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        }

        [Fact]
        public async Task UpdateAvatarAsync_UpdatedUserAvatar_WhenPassedIdIsValid()
        {
            //Arrange
            var id = _fixture.Create<Guid>();
            var user = _fixture.Build<User>()
                .With(u => u.Id, id)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            var avatarDto = _fixture.Create<AvatarChangingDto>();
            var repositoryMock = new Mock<IRepositoryManager>();
            repositoryMock.Setup(r => r.UserRepository.GetUserByIdAsync(id, true))
                .Returns(Task.FromResult(user));
            repositoryMock.Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);
            var avatarServiceMock = new Mock<IAvatarService>();
            avatarServiceMock.Setup(s => s.CreateImageAsync(id, avatarDto))
                .Returns(Task.FromResult(_fixture.Create<string>()));
            avatarServiceMock.Setup(s => s.DeleteOldImage(It.IsAny<string>()));

            var service = new UserService(repositoryMock.Object, null, null, avatarServiceMock.Object);

            //Act
            await service.UpdateAvatarAsync(id, avatarDto);

            //Assert
            avatarServiceMock.Verify(a => a.CreateImageAsync(id, avatarDto), Times.Once);
            avatarServiceMock.Verify(a => a.DeleteOldImage(It.IsAny<string>()), Times.Once);
            repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAvatarAsync_ThrowingNotFoundException_WhenPassedIdIsInvalid()
        {
            //Arrange
            var id = _fixture.Create<Guid>();
            var repositoryMock = new Mock<IRepositoryManager>();
            repositoryMock.Setup(r => r.UserRepository.GetUserByIdAsync(id, false))
                .Returns(Task.FromResult<User>(null));

            var service = new UserService(repositoryMock.Object, null, null, null);

            //Act
            Func<Task> result = async () => await service.GetUserInfoForLayoutByIdAsync(id);

            //Assert
            await result.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        }
    }
}
