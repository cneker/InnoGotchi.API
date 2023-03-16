using AutoFixture;
using FluentAssertions;
using Infrastructure.Persistance;
using InnoGotchi.Application.DataTransferObjects.User;
using InnoGotchi.Domain.Entities;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace InnoGotchi.IntegrationTests
{
    public class UserControllerTests : IClassFixture<WebApplicationFixture>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFixture _webApplicationFixture;
        private readonly IFixture _fixture;
        private readonly AppDbContext _context;
        private const string Base64Image = "base64string";
        public UserControllerTests(WebApplicationFixture applicationFixture)
        {
            _fixture = new Fixture();
            _webApplicationFixture = applicationFixture;
            _context = _webApplicationFixture.Context;
            _client = _webApplicationFixture.Client;
        }

        private void AddUsersToDb(IEnumerable<User> users)
        {
            _context.Users.AddRange(users);
            _context.SaveChanges();
        }

        private void AddUsersToDb(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        private void RemoveUsersFromDb()
        {
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetUsers_WhenThereIsNoRecordsInUsersTable_ReturnsOkStatusCodeAndEmptyList()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("api/users");
            var users = await response.Content.ReadAsAsync<IEnumerable<UserInfoDto>>();

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.OK);
            users.Should().NotBeNull()
                .And.BeEmpty();
        }

        [Fact]
        public async Task GetUsers_WhenThreeRecordsExistInUsersTable_ReturnsOkStatusCodeAndListWithTHreeItems()
        {
            // Arrange
            var users = _fixture.Build<User>()
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .CreateMany(3);
            AddUsersToDb(users);

            // Act
            var response = await _client.GetAsync("api/users");
            var responseContent = await response.Content.ReadAsAsync<IEnumerable<UserInfoDto>>();
            RemoveUsersFromDb();

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.OK);
            responseContent.Should().NotBeNull()
                .And.NotBeEmpty()
                .And.HaveCount(3)
                .And.AllBeOfType<UserInfoDto>();
        }

        [Theory]
        [InlineData("test@mail.com", "TESTtest123*", "FirstName", "LastName")]
        [InlineData("onemore@gmail.com", "Asd12d*", "Steeeveeee", "Eeeeveeeets")]
        public async Task CreateUser_WhenModelIsValid_ReturnsCreatedStatusCodeWithLocationHeaderAndUserObject(
            string email, string password, string fName, string lName)
        {
            // Arrange
            var userForReg = new UserForRegistrationDto
            {
                Email = email,
                Password = password,
                FirstName = fName,
                LastName = lName
            };
            var httpContent = new StringContent(JsonConvert.SerializeObject(userForReg), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("api/users", httpContent);
            var user = await response.Content.ReadAsAsync<UserInfoDto>();

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.Created);
            response.Headers.FirstOrDefault(h => h.Key == "Location").Should().NotBeNull();
            user.Email.Should().BeEquivalentTo(userForReg.Email);
            user.FirstName.Should().BeEquivalentTo(userForReg.FirstName);
            user.LastName.Should().BeEquivalentTo(userForReg.LastName);

            RemoveUsersFromDb();
        }

        [Theory]
        [InlineData("testmail.com", "TESTtest123*", "FirstName", "LastName")]
        [InlineData("test@mail.com", "test123", "FirstName", "LastName")]
        [InlineData("test@mail.com", "TESTtest123*", "", "LastName")]
        [InlineData("onemore@gmail.com", "Asd12d*", "Steeeveeee", "VvvvveeeeerrrrryyyyyyyLargeLastName")]
        public async Task CreateUser_WhenModelIsInvalid_ReturnsBadRequestStatusCode(
            string email, string password, string fName, string lName)
        {
            // Arrange
            var userForReg = new UserForRegistrationDto
            {
                Email = email,
                Password = password,
                FirstName = fName,
                LastName = lName
            };
            var httpContent = new StringContent(JsonConvert.SerializeObject(userForReg), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("api/users", httpContent);

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetUser_WhenJWTAndPassedIdAreValid_ReturnsOkStatusCodeAndUserItem()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            AddUsersToDb(user);
            var jwt = _webApplicationFixture.GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            // Act
            var response = await _client.GetAsync($"api/users/{user.Id}");
            var responseContent = await response.Content.ReadAsAsync<UserInfoDto>();

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.OK);
            responseContent.Should().NotBeNull()
                .And.BeOfType<UserInfoDto>();
            responseContent.Id.Should().Be(user.Id);

            RemoveUsersFromDb();
            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task GetUser_WhenJWTIsInvalidAndPassedIdIsValid_ReturnsUnauthorizedStatusCode()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            AddUsersToDb(user);
            var invalidJWT = _fixture.Create<string>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", invalidJWT);

            // Act
            var response = await _client.GetAsync($"api/users/{user.Id}");

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);

            RemoveUsersFromDb();
            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task GetUser_WhenJWTIsValidAndPassedIdIsInvalid_ReturnsForbiddenStatusCode()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            var user = _fixture.Build<User>()
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            AddUsersToDb(user);
            var jwt = _webApplicationFixture.GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            // Act
            var response = await _client.GetAsync($"api/users/{invalidId}");

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.Forbidden);

            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task UpdateUserInfo_WhenJWTAndPassedIdAndModelAreValid_ReturnsNoContentStatusCode()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            AddUsersToDb(user);
            var model = new UserInfoForUpdateDto 
            { 
                FirstName = "FirstName", 
                LastName = "LastName" 
            };
            var jwt = _webApplicationFixture.GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var httpContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"api/users/{user.Id}", httpContent);

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.NoContent);
            response = await _client.GetAsync($"api/users/{user.Id}");
            var userResult = await response.Content.ReadAsAsync<UserInfoDto>();
            userResult.FirstName.Should().BeEquivalentTo(model.FirstName);
            userResult.LastName.Should().BeEquivalentTo(model.LastName);

            RemoveUsersFromDb();
            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task UpdateUserInfo_WhenJWTIsInvalidAndPassedIdAndodelAreValid_ReturnsUnathorizedStatusCode()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            AddUsersToDb(user);
            var model = new UserInfoForUpdateDto
            {
                FirstName = "FirstName",
                LastName = "LastName"
            };
            var invalidJWT = _fixture.Create<string>();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", invalidJWT);
            var httpContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"api/users/{user.Id}", httpContent);

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);

            RemoveUsersFromDb();
            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task UpdateUserInfo_WhenJWTAndPassedModelAreValidAndPassedIdIsInvalid_ReturnsForbiddenStatusCode()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            var user = _fixture.Build<User>()
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            AddUsersToDb(user);
            var model = new UserInfoForUpdateDto
            {
                FirstName = "FirstName",
                LastName = "LastName"
            };
            var jwt = _webApplicationFixture.GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var httpContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"api/users/{invalidId}", httpContent);

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.Forbidden);

            RemoveUsersFromDb();
            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task UpdateUserInfo_WhenJwtAndPassedIdAreValidAndPassedModelIsInvalid_ReturnsStatusCode()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            AddUsersToDb(user);
            var model = _fixture.Create<UserInfoForUpdateDto>();
            var jwt = _webApplicationFixture.GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var httpContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"api/users/{user.Id}", httpContent);

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            RemoveUsersFromDb();
            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task ChangeUserPassword_WhenJWTAndPassedIdAndModelAreValid_ReturnsNoContentStatusCode()
        {
            // Arrange
            var oldPassword = "Pass123*";
            var newPassword = "Password123*";
            var user = _fixture.Build<User>()
                .With(u => u.PasswordHash, _webApplicationFixture.GeneratePasswordHash(oldPassword))
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            AddUsersToDb(user);
            var model = new PasswordChangingDto
            {
                OldPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmedPassword = newPassword
            };
            var jwt = _webApplicationFixture.GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var httpContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"api/users/{user.Id}/change-password", httpContent);

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.NoContent);

            RemoveUsersFromDb();
            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task ChangeUserPassword_WhenJWTIsInvalidAndPassedIdAndModelAreValid_ReturnsUnauthorizedStatusCode()
        {
            // Arrange
            var oldPassword = "Pass123*";
            var newPassword = "Password123*";
            var user = _fixture.Build<User>()
                .With(u => u.PasswordHash, _webApplicationFixture.GeneratePasswordHash(oldPassword))
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            AddUsersToDb(user);
            var model = new PasswordChangingDto
            {
                OldPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmedPassword = newPassword
            };
            var invalidJWT = _fixture.Create<string>();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", invalidJWT);
            var httpContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"api/users/{user.Id}/change-password", httpContent);

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);

            RemoveUsersFromDb();
            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task ChangeUserPassword_WhenJWTAndPassedModelAreValidAndPassedIdIsInvalid_ReturnsForbiddenStatusCode()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            var oldPassword = "Pass123*";
            var newPassword = "Password123*";
            var user = _fixture.Build<User>()
                .With(u => u.PasswordHash, _webApplicationFixture.GeneratePasswordHash(oldPassword))
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            AddUsersToDb(user);
            var model = new PasswordChangingDto
            {
                OldPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmedPassword = newPassword
            };
            var jwt = _webApplicationFixture.GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var httpContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"api/users/{invalidId}/change-password", httpContent);

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.Forbidden);

            RemoveUsersFromDb();
            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Theory]
        [InlineData("Pass123*", "Pass123*", "Pass123*")]
        [InlineData("Pass123*", "Password1*", "Password123*")]
        [InlineData("Pas13*", "Password123*", "Password123*")]
        public async Task ChangeUserPassword_WhenJWTAndPassedIdAreValidAndPassedModelIsInvalid_ReturnsBadRequestStatusCode(
            string oldPassword, string newPassword, string confirmedPassword)
        {
            // Arrange
            var userPassword = "Pass123*";
            var user = _fixture.Build<User>()
                .With(u => u.PasswordHash, _webApplicationFixture.GeneratePasswordHash(userPassword))
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            AddUsersToDb(user);
            var model = new PasswordChangingDto
            {
                OldPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmedPassword = confirmedPassword
            };
            var jwt = _webApplicationFixture.GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var httpContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"api/users/{user.Id}/change-password", httpContent);

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            RemoveUsersFromDb();
            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task DeleteUser_WhenPassedIdIsValidAndUserExistsInTheDatabase_ReturnsNoContentStatusCode()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            AddUsersToDb(user);

            // Act
            var response = await _client.DeleteAsync($"api/users/{user.Id}");

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.NoContent);

            RemoveUsersFromDb();
        }

        [Fact]
        public async Task DeleteUser_WhenPassedIdIsInvalidAndUserExistsInTheDatabase_ReturnsNotFoundStatusCode()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            var user = _fixture.Build<User>()
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            AddUsersToDb(user);

            // Act
            var response = await _client.DeleteAsync($"api/users/{invalidId}");

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.NotFound);

            RemoveUsersFromDb();
        }

        [Fact]
        public async Task DeleteUser_WhenPassedIdIsInvalidAndUserDoesntExistInTheDatabase_ReturnsNotFoundStatusCode()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"api/users/{invalidId}");

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.NotFound);

            RemoveUsersFromDb();
        }

        [Fact]
        public async Task UpdateAvatarAsync_WhenJWTAndPassedIdAndModelAreValid_ReturnsNoContentStatusCode()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            AddUsersToDb(user);
            var model = new AvatarChangingDto
            {
                Base64Image = Base64Image,
                FileName = "test_image.jpg"
            };
            var jwt = _webApplicationFixture.GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var httpContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"api/users/{user.Id}/update-avatar", httpContent);

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.NoContent);

            RemoveUsersFromDb();
            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task UpdateAvatarAsync_WhenJWTIsInvalidAndPassedIdAndModelAreValid_ReturnsUnauthorizedStatusCode()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            AddUsersToDb(user);
            var model = new AvatarChangingDto
            {
                Base64Image = Base64Image,
                FileName = "test_image.jpg"
            };
            var invalidJWT = _fixture.Create<string>();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", invalidJWT);
            var httpContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"api/users/{user.Id}/update-avatar", httpContent);

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);

            RemoveUsersFromDb();
            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task UpdateAvatarAsync_WhenJWTAndPassedModelAreValidAndPassedIdIsInvalid_ReturnsForbiddenStatusCode()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            var user = _fixture.Build<User>()
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            AddUsersToDb(user);
            var model = new AvatarChangingDto
            {
                Base64Image = Base64Image,
                FileName = "test_image.jpg"
            };
            var jwt = _webApplicationFixture.GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var httpContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"api/users/{invalidId}/update-avatar", httpContent);

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.Forbidden);

            RemoveUsersFromDb();
            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task UpdateAvatarAsync_WhenJWTAndPassedIdAreValidAndPassedModelAIsInvalid_ReturnsBadRequestStatusCode()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .Without(u => u.Role)
                .Without(u => u.AvatarPath)
                .Without(u => u.UserFarm)
                .Without(u => u.FriendsFarms)
                .Create();
            AddUsersToDb(user);
            var model = new AvatarChangingDto
            {
                Base64Image = Base64Image,
                FileName = "file"
            };
            var jwt = _webApplicationFixture.GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var httpContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"api/users/{user.Id}/update-avatar", httpContent);

            // Assert
            response.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            RemoveUsersFromDb();
            _client.DefaultRequestHeaders.Authorization = null;
        }
    }
}
