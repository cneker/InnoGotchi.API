using InnoGotchi.API.Filters.ActionFilters;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Farm;
using InnoGotchi.Application.DataTransferObjects.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoGotchi.API.Controllers
{
    [Route("api/users/{userId}/farm")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiController]
    public class FarmsController : ControllerBase
    {
        private readonly IFarmService _farmService;
        private readonly ILogger _logger;
        public FarmsController(IFarmService farmService, ILogger<FarmsController> logger)
        {
            _farmService = farmService;
            _logger = logger;
        }

        /// <summary>
        /// Get a list of all farms
        /// </summary>
        /// <returns>A farms list</returns>
        /// <response code="200">Returns a list of all farms</response>
        [HttpGet("/api/farms")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFarms()
        {
            var farmsDto = await _farmService.GetFarmsOverviewAsync();
            _logger.LogInformation("Send all farms");
            return Ok(farmsDto);
        }

        /// <summary>
        /// Get a farm
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>An existing farm item</returns>
        /// <response code="200">Returns an existing farm item</response>
        /// <response code="404">If user doesn't have a farm</response>
        [HttpGet(Name = "GetFarmOverview"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFarmOverview(Guid userId)
        {
            var farmDto = await _farmService.GetFarmOverviewByIdAsync(userId);
            _logger.LogInformation("Send FarmOverview");
            return Ok(farmDto);
        }

        /// <summary>
        /// Create a new farm for the user
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="farmDto">Model for creating a farm</param>
        /// <returns>A newly created farm</returns>
        /// <response code="201">Returns the newly created farm</response>
        /// <response code="403">If you try to make an action as another user</response>
        /// <response code="400">If user already has a farm or passed farm name has alredy registered</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ServiceFilter(typeof(CheckUserIdAttribute)), Authorize]
        public async Task<IActionResult> CreateFarm(Guid userId, [FromBody] FarmForCreationDto farmDto)
        {
            var farm = await _farmService.CreateFarmAsync(userId, farmDto);
            _logger.LogInformation("Farm was created");
            return CreatedAtRoute("GetFarmOverview", new { userId }, farm);
        }

        /// <summary>
        /// Get detail information about the farm
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="farmId">Farm Id</param>
        /// <returns>A detailed information item</returns>
        /// <response code="200">Returns detailed information about the farm</response>
        /// <responce code="404">If the farm Id is invalid or the farm doesn't exist</responce>
        /// <response code="403">If the user isn't the owner or the collaborator of the farm or if you try to make an action as another user</response>
        [HttpGet("{farmId}/detail"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> GetFarmDetails(Guid userId, Guid farmId)
        {
            var farmDto = await _farmService.GetFarmDetailsByFarmIdAsync(userId, farmId);
            _logger.LogInformation("Send FarmDetails");
            return Ok(farmDto);
        }

        /// <summary>
        /// Get statistics about the farms
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="farmId">Farm Id</param>
        /// <returns>A statistics item</returns>
        /// <response code="200">Returns the statistics about the farm</response>
        /// <responce code="404">If the farm Id is invalid or the farm doesn't exist</responce>
        /// <response code="403">If the user isn't the owner of the farm or if you try to make an action as another user</response>
        [HttpGet("{farmId}/statistics"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> GetFarmStatistics(Guid userId, Guid farmId)
        {
            var farmDto = await _farmService.GetFarmStatisticsByFarmIdAsync(userId, farmId);
            _logger.LogInformation("Send FarmStatistics");
            return Ok(farmDto);
        }

        /// <summary>
        /// Get a list of friends of the farm
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>A list of users</returns>
        /// <response code="200">Returns a list of friends of the farm</response>
        /// <response code="403">If you try to make an action as another user</response>
        [HttpGet("friends"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> GetFriendsFarms(Guid userId)
        {
            var friends = await _farmService.GetFriendsFarmsAsync(userId);
            _logger.LogInformation("Send friends farms");
            return Ok(friends);
        }

        /// <summary>
        /// Set a friend to be a collaborator of your farm
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="farmId">Farm Id</param>
        /// <param name="userDto">Model with friend's email</param>
        /// <returns>Nothing</returns>
        /// <response code="204">If user is successfully invited</response>
        /// <response code="404">If farm Id is invalid</response>
        /// <response code="403">If the user isn't the owner of the farm or if you try to make an action as another user</response>
        /// <response code="400">If the friend is already your friend or you try to invite yourself as a friend</response>
        [HttpPost("{farmId}/invite-collaborator"), Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> InviteCollaborator(Guid userId, Guid farmId, [FromBody] UserForInvitingDto userDto)
        {
            await _farmService.InviteFriendAsync(userId, farmId, userDto);
            _logger.LogInformation("Collaborator was invited");
            return NoContent();
        }

        /// <summary>
        /// Update an information about the farm
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="farmId">Farm Id</param>
        /// <param name="farmDto">Model with updated information</param>
        /// <returns>Nothing</returns>
        /// <response code="204">If farm is successfully updated</response>
        /// <response code="404">If farm Id is invalid</response>
        /// <response code="403">If the user isn't the owner of the farm or if you try to make an action as another user</response>
        /// <response code="400">If this farm name has already registered</response>
        [HttpPut("{farmId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(CheckUserIdAttribute)), Authorize]
        public async Task<IActionResult> UpdateFarm(Guid userId, Guid farmId, [FromBody] FarmForUpdateDto farmDto)
        {
            await _farmService.UpdateFarmNameAsync(userId, farmId, farmDto);
            _logger.LogInformation("Farm was updated");
            return NoContent();
        }

        /// <summary>
        /// Delete the farm
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>Nothing</returns>
        /// <response code="204">If farm is successfully deleted</response>
        /// <response code="404">If user Id is invalid</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFarm(Guid userId)
        {
            await _farmService.DeleteFarmById(userId);
            _logger.LogInformation("Farm was deleted");
            return NoContent();
        }
    }
}
