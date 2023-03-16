using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.User;
using Microsoft.AspNetCore.Mvc;

namespace InnoGotchi.API.Controllers
{
    [Route("api/authentication")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly ILogger _logger;

        public AuthenticationController(IAuthenticationService authService, ILogger<AuthenticationController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Sign in to the application
        /// </summary>
        /// <param name="userDto">User credentials</param>
        /// <returns>JWT token and user Id</returns>
        /// <response code="200">Returns JWT token and user Id</response>
        /// <response code="400">If credentials are wrong</response>
        [HttpPost(Name = "SignIn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignIn([FromBody] UserForAuthenticationDto userDto)
        {
            var token = await _authService.SignInAsync(userDto);
            _logger.LogInformation("Send JWT");
            return Ok(token);
        }
    }
}
