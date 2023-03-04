using Infrastructure.Persistance;
using Infrastructure.Persistance.Repositories;
using InnoGotchi.Domain.Entities;

namespace Tests.Repositories
{
    public class UserRepositoryTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly Fixture _fixture;
        private readonly AppDbContext _context;
        private readonly UserRepository _repository;
        public UserRepositoryTests(TestDatabaseFixture fixture)
        {
            _fixture = new Fixture();
            _context = fixture.Context;
            _repository = new UserRepository(_context);
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsUsersList()
        {
            //Arrange

            //Act
            var result = await _repository.GetUsersAsync(false);

            //Assert
            result.Should().NotBeNull()
                .And.HaveCount(3);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserExistsAndPassedIdIsValid_ReturnsUser()
        {
            //Arrange
            var userId = Guid.Parse("8cd6c906-81c5-4717-8428-bfebe34d0f09");

            //Act
            var result = await _repository.GetUserByIdAsync(userId, false);

            //Assert
            result.Should().NotBeNull()
                .And.BeOfType<User>();
            result.Id.Should().Be(userId);
            result.UserFarm.Should().NotBeNull();
            result.FriendsFarms.Should().NotBeNull();
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenPassedIdIsInvalid_ReturnsNull()
        {
            //Arrange
            var userId = _fixture.Create<Guid>();

            //Act
            var result = await _repository.GetUserByIdAsync(userId, false);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUserByEmailAsync_WhenUserExistsAndPassedEmailIsValid_ReturnsUser()
        {
            //Arrange
            var userEmail = "email1@mail.com";

            //Act
            var result = await _repository.GetUserByEmailAsync(userEmail, false);

            //Assert
            result.Should().NotBeNull()
                .And.BeOfType<User>();
            result.Email.Should().Be(userEmail);
            result.UserFarm.Should().NotBeNull();
            result.FriendsFarms.Should().NotBeNull();
        }

        [Fact]
        public async Task GetUserByEmailAsync_WhenPassedEmailIsInvalid_ReturnsNull()
        {
            //Arrange
            var userEmail = _fixture.Create<string>();

            //Act
            var result = await _repository.GetUserByEmailAsync(userEmail, false);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateUser_SavedUser()
        {
            //Arrange
            var user = _fixture.Build<User>()
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            _context.Database.BeginTransaction();

            //Act
            await _repository.CreateUser(user);
            _context.SaveChanges();

            //Assert
            var result = await _repository.GetUserByIdAsync(user.Id, false);
            _context.Database.RollbackTransaction();
            result.Should().NotBeNull()
                .And.Match<User>(u => u.Id == user.Id);
        }

        [Fact]
        public async Task DeleteUser_DeletedUser()
        {
            //Arrange
            var user = _context.Users.Where(u => u.Email == "email2@mail.com").Single();
            _context.Database.BeginTransaction();

            //Act
            _repository.DeleteUser(user);
            _context.SaveChanges(true);

            //Assert
            var result = await _repository.GetUserByIdAsync(user.Id, false);
            _context.Database.RollbackTransaction();
            result.Should().BeNull();
        }
    }
}
