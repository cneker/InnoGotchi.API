﻿using InnoGotchi.API.Filters.ActionFilters;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.User;
using Microsoft.AspNetCore.Mvc;

namespace InnoGotchi.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var usersDto = await _userService.GetUsersInfoAsync();

            return Ok(usersDto);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var userDto = await _userService.GetUserInfoByIdAsync(id);

            return Ok(userDto);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateUser([FromBody] UserForRegistrationDto userDto)
        {
            var user = await _userService.CreateUserAsync(userDto);

            return CreatedAtRoute("GetUser", new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateUserInfo(Guid id, [FromBody] UserInfoForUpdateDto userDto)
        {
            await _userService.UpdateUserInfoAsync(id, userDto);

            return NoContent();
        }

        [HttpPut("{id}/change-password")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
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