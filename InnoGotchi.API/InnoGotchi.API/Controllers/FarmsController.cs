using InnoGotchi.API.Filters.ActionFilters;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Farm;
using InnoGotchi.Application.DataTransferObjects.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoGotchi.API.Controllers
{
    [Route("api/users/{id}/farm")]
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
        public async Task<IActionResult> GetFarmOverview(Guid id)
        {
            var farmDto = await _farmService.GetFarmOverviewByIdAsync(id);
            _logger.LogInformation("Send FarmOverview");
            return Ok(farmDto);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute)), Authorize]
        public async Task<IActionResult> CreateFarm(Guid id, [FromBody] FarmForCreationDto farmDto)
        {
            var farm = await _farmService.CreateFarmAsync(id, farmDto);
            _logger.LogInformation("Farm was created");
            return CreatedAtRoute("GetFarmOverview", new { id }, farm);
        }

        [HttpGet("{farmId}/detail"), Authorize]
        public async Task<IActionResult> GetFarmDetails(Guid id, Guid farmId)
        {
            var farmDto = await _farmService.GetFarmDetailsByFarmIdAsync(id, farmId);
            _logger.LogInformation("Send FarmDetails");
            return Ok(farmDto);
        }

        [HttpGet("{farmId}/statistics"), Authorize]
        public async Task<IActionResult> GetFarmStatistics(Guid id, Guid farmId)
        {
            var farmDto = await _farmService.GetFarmStatisticsByFarmIdAsync(id, farmId);
            _logger.LogInformation("Send FarmStatistics");
            return Ok(farmDto);
        }

        [HttpGet("friends"), Authorize]
        public async Task<IActionResult> GetFriendsFarms(Guid id)
        {
            var friends = await _farmService.GetFriendsFarmsAsync(id);
            _logger.LogInformation("Send friends farms");
            return Ok(friends);
        }

        [HttpPost("collaborators"), Authorize]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> InviteCollaborator(Guid id, [FromBody] UserForInvitingDto userDto)
        {
            await _farmService.InviteFriendAsync(id, userDto);
            _logger.LogInformation("Collaborator was invited");
            return NoContent();
        }

        [HttpPut]
        [ServiceFilter(typeof(ValidationFilterAttribute)), Authorize]
        public async Task<IActionResult> UpdateFarm(Guid id, [FromBody] FarmForUpdateDto farmDto)
        {
            await _farmService.UpdateFarmNameAsync(id, farmDto);
            _logger.LogInformation("Farm was updated");
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFarm(Guid id)
        {
            await _farmService.DeleteFarmById(id);
            _logger.LogInformation("Farm was deleted");
            return NoContent();
        }
    }
}
