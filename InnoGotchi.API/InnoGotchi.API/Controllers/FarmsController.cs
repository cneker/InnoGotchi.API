using InnoGotchi.API.Filters.ActionFilters;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Farm;
using InnoGotchi.Application.DataTransferObjects.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoGotchi.API.Controllers
{
    [Route("api/users/{userId}/farm")]
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

        [HttpGet("/api/farms")]
        public async Task<IActionResult> GetFarms()
        {
            var farmsDto = await _farmService.GetFarmsOverviewAsync();
            _logger.LogInformation("Send all farms");
            return Ok(farmsDto);
        }

        [HttpGet(Name = "GetFarmOverview"), Authorize]
        public async Task<IActionResult> GetFarmOverview(Guid userId)
        {
            var farmDto = await _farmService.GetFarmOverviewByIdAsync(userId);
            _logger.LogInformation("Send FarmOverview");
            return Ok(farmDto);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute)), Authorize]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> CreateFarm(Guid userId, [FromBody] FarmForCreationDto farmDto)
        {
            var farm = await _farmService.CreateFarmAsync(userId, farmDto);
            _logger.LogInformation("Farm was created");
            return CreatedAtRoute("GetFarmOverview", new { userId }, farm);
        }

        [HttpGet("{farmId}/detail"), Authorize]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> GetFarmDetails(Guid userId, Guid farmId)
        {
            var farmDto = await _farmService.GetFarmDetailsByFarmIdAsync(userId, farmId);
            _logger.LogInformation("Send FarmDetails");
            return Ok(farmDto);
        }

        [HttpGet("{farmId}/statistics"), Authorize]
        public async Task<IActionResult> GetFarmStatistics(Guid userId, Guid farmId)
        {
            var farmDto = await _farmService.GetFarmStatisticsByFarmIdAsync(userId, farmId);
            _logger.LogInformation("Send FarmStatistics");
            return Ok(farmDto);
        }

        [HttpGet("friends"), Authorize]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> GetFriendsFarms(Guid userId)
        {
            var friends = await _farmService.GetFriendsFarmsAsync(userId);
            _logger.LogInformation("Send friends farms");
            return Ok(friends);
        }

        [HttpPost("{farmId}/invite-collaborator"), Authorize]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> InviteCollaborator(Guid userId, Guid farmId, [FromBody] UserForInvitingDto userDto)
        {
            await _farmService.InviteFriendAsync(userId, farmId, userDto);
            _logger.LogInformation("Collaborator was invited");
            return NoContent();
        }

        [HttpPut("{farmId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute)), Authorize]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> UpdateFarm(Guid userId, Guid farmId, [FromBody] FarmForUpdateDto farmDto)
        {
            await _farmService.UpdateFarmNameAsync(userId, farmId, farmDto);
            _logger.LogInformation("Farm was updated");
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFarm(Guid userId)
        {
            await _farmService.DeleteFarmById(userId);
            _logger.LogInformation("Farm was deleted");
            return NoContent();
        }
    }
}
