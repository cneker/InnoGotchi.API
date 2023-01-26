﻿using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.User;
using Microsoft.AspNetCore.Mvc;

namespace InnoGotchi.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var usersDto = await _userService.GetUsersInfoAsync();

            return Ok(usersDto);
        }

        [HttpGet("profile/{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var userDto = await _userService.GetUserInfoByIdAsync(id);

            return Ok(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserForRegistrationDto userDto)
        {
            var userId = await _userService.CreateUserAsync(userDto);

            return CreatedAtRoute("GetUser", new { id = userId }, userId);
        }

        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateUserInfo(Guid id, [FromBody] UserInfoForUpdateDto userDto)
        {
            await _userService.UpdateUserInfoAsync(id, userDto);

            return NoContent();
        }

        [HttpPut("profile/change-password/{id}")]
        public async Task<IActionResult> ChangeUserPassword(Guid id, [FromBody] PasswordChangingDto passwordDto)
        {
            await _userService.UpdatePasswordAsync(id, passwordDto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUserById(id);

            return NoContent();
        }
    }
}