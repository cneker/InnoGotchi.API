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

        [HttpGet("{id}/layout"), Authorize]
        public async Task<IActionResult> GetUserForLayout(Guid id)
        {
            var userDto = await _userService.GetUserInfoForLayoutByIdAsync(id);
            _logger.LogInformation("Send user for layout");
            return Ok(userDto);
        }

        [HttpGet("{id}", Name = "GetUser"), Authorize]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var userDto = await _userService.GetUserInfoByIdAsync(id);
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

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute)), Authorize]
        public async Task<IActionResult> UpdateUserInfo(Guid id, [FromBody] UserInfoForUpdateDto userDto)
        {
            await _userService.UpdateUserInfoAsync(id, userDto);
            _logger.LogInformation("User was updated");
            return NoContent();
        }

        [HttpPut("{id}/change-password")]
        [ServiceFilter(typeof(ValidationFilterAttribute)), Authorize]
        public async Task<IActionResult> ChangeUserPassword(Guid id, [FromBody] PasswordChangingDto passwordDto)
        {
            await _userService.UpdatePasswordAsync(id, passwordDto);
            _logger.LogInformation("Password was changed");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUserById(id);
            _logger.LogInformation("User was deleted");
            return NoContent();
        }

        [HttpPut("{id}/update-avatar"), Authorize]
        public async Task<IActionResult> UpdateAvatar(Guid id, AvatarChangingDto avatarDto)
        {
            await _userService.UpdateAvatarAsync(id, avatarDto);
            _logger.LogInformation("Avatar was updated");
            return NoContent();
        }
    }
}
