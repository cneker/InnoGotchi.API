using InnoGotchi.API.Filters.ActionFilters;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoGotchi.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger _logger;
        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var usersDto = await _userService.GetUsersInfoAsync();
            _logger.LogInformation("Send all users");
            return Ok(usersDto);
        }

        [HttpGet("{userId}", Name = "GetUser"), Authorize]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            var userDto = await _userService.GetUserInfoByIdAsync(userId);
            _logger.LogInformation("Send user");
            return Ok(userDto);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateUser([FromBody] UserForRegistrationDto userDto)
        {
            var user = await _userService.CreateUserAsync(userDto);
            _logger.LogInformation("User was created");
            return CreatedAtRoute("SignIn", new { Controller = "Authentication" }, user);
        }

        [HttpPut("{userId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute)), Authorize]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> UpdateUserInfo(Guid userId, [FromBody] UserInfoForUpdateDto userDto)
        {
            await _userService.UpdateUserInfoAsync(userId, userDto);
            _logger.LogInformation("User was updated");
            return NoContent();
        }

        [HttpPut("{userId}/change-password")]
        [ServiceFilter(typeof(ValidationFilterAttribute)), Authorize]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> ChangeUserPassword(Guid userId, [FromBody] PasswordChangingDto passwordDto)
        {
            await _userService.UpdatePasswordAsync(userId, passwordDto);
            _logger.LogInformation("Password was changed");
            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            await _userService.DeleteUserById(userId);
            _logger.LogInformation("User was deleted");
            return NoContent();
        }

        [HttpPut("{userId}/update-avatar"), Authorize]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> UpdateAvatar(Guid userId, AvatarChangingDto avatarDto)
        {
            await _userService.UpdateAvatarAsync(userId, avatarDto);
            _logger.LogInformation("Avatar was updated");
            return NoContent();
        }
    }
}
