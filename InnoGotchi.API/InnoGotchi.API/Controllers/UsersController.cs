using InnoGotchi.API.Filters.ActionFilters;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoGotchi.API.Controllers
{
    [Route("api/users")]
    [Consumes("application/json")]
    [Produces("application/json")]
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

        /// <summary>
        /// Get a list of all users
        /// </summary>
        /// <returns>A list of all users</returns>
        /// <response code="200">Returns a list of all users</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers()
        {
            var usersDto = await _userService.GetUsersInfoAsync();
            _logger.LogInformation("Send all users");
            return Ok(usersDto);
        }

        /// <summary>
        /// Get a user
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>An existing user item</returns>
        /// <response code="200">Returns an existing user item</response>
        /// <response code="403">If you try to make an action as another user</response>
        /// <response code="401">If your jwt is invalid</response>
        [HttpGet("{userId}", Name = "GetUser"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            var userDto = await _userService.GetUserInfoByIdAsync(userId);
            _logger.LogInformation("Send user");
            return Ok(userDto);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="userDto">Model for creating a user</param>
        /// <returns>A newly created user</returns>
        /// <response code="201">Returns a newly created user</response>
        /// <response code="400">If passed email has alredy registered</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] UserForRegistrationDto userDto)
        {
            var user = await _userService.CreateUserAsync(userDto);
            _logger.LogInformation("User was created");
            return CreatedAtRoute("SignIn", new { Controller = "Authentication" }, user);
        }

        /// <summary>
        /// Update information about a user
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="userDto">Model with updated information</param>
        /// <returns>Nothing</returns>
        /// <response code="204">If a user is successfully updated</response>
        /// <response code="403">If you try to make an action as another user</response>
        /// <response code="400">If passed model is invalid</response>
        /// <response code="401">If your jwt is invalid</response>
        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(CheckUserIdAttribute)), Authorize]
        public async Task<IActionResult> UpdateUserInfo(Guid userId, [FromBody] UserInfoForUpdateDto userDto)
        {
            await _userService.UpdateUserInfoAsync(userId, userDto);
            _logger.LogInformation("User was updated");
            return NoContent();
        }

        /// <summary>
        /// Change a password
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="passwordDto">Model for change a password</param>
        /// <returns>Nothing</returns>
        /// <response code="204">If a password is successfully changed</response>
        /// <response code="403">If you try to make an action as another user</response>
        /// <response code="400">If passed model is invalid</response>
        /// <response code="401">If your jwt is invalid</response>
        [HttpPut("{userId}/change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(CheckUserIdAttribute)), Authorize]
        public async Task<IActionResult> ChangeUserPassword(Guid userId, [FromBody] PasswordChangingDto passwordDto)
        {
            await _userService.UpdatePasswordAsync(userId, passwordDto);
            _logger.LogInformation("Password was changed");
            return NoContent();
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Nothing</returns>
        /// <response code="204">If a user is successfully deleted</response>
        /// <response code="404">If the user Id is invalid</response>
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            await _userService.DeleteUserByIdAsync(userId);
            _logger.LogInformation("User was deleted");
            return NoContent();
        }

        /// <summary>
        /// Update user avatar
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="avatarDto">Model with base64 image string</param>
        /// <returns>Nothing</returns>
        /// <response code="204">If an avatar is successfully updated</response>
        /// <response code="403">If you try to make an action as another user</response>
        /// <response code="400">If passed model is invalid</response>
        /// <response code="401">If your jwt is invalid</response>
        [HttpPut("{userId}/update-avatar"), Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> UpdateAvatar(Guid userId, AvatarChangingDto avatarDto)
        {
            await _userService.UpdateAvatarAsync(userId, avatarDto);
            _logger.LogInformation("Avatar was updated");
            return NoContent();
        }
    }
}
