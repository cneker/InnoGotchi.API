using InnoGotchi.API.Filters.ActionFilters;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Farm;
using InnoGotchi.Application.DataTransferObjects.User;
using Microsoft.AspNetCore.Mvc;

namespace InnoGotchi.API.Controllers
{
    [Route("api/users/{userId}/farms")]
    [ApiController]
    public class FarmsController : ControllerBase
    {
        private readonly IFarmService _farmService;
        public FarmsController(IFarmService farmService)
        {
            _farmService = farmService;
        }

        [HttpGet("/api/farms")]
        public async Task<IActionResult> GetFarms()
        {
            var farmsDto = await _farmService.GetFarmsOverviewAsync();

            return Ok(farmsDto);
        }

        [HttpGet(Name = "GetFarmOverview")]
        public async Task<IActionResult> GetFarmOverview(Guid userId)
        {
            var farmDto = await _farmService.GetFarmOverviewByIdAsync(userId);

            return Ok(farmDto);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateFarm(Guid userId, [FromBody] FarmForCreationDto farmDto)
        {
            var farmId = await _farmService.CreateFarmAsync(userId, farmDto);

            return CreatedAtRoute("GetFarmOverview", new { userId = userId }, farmId);
        }

        [HttpGet("detail")]
        public async Task<IActionResult> GetFarmDetails(Guid userId)
        {
            var farmDto = await _farmService.GetFarmDetailsByIdAsync(userId);

            return Ok(farmDto);
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetFarmStatistics(Guid userId)
        {
            var farmDto = await _farmService.GetFarmStatisticsByIdAsync(userId);

            return Ok(farmDto);
        }

        [HttpGet("friends")]
        public async Task<IActionResult> GetFriendsFarms(Guid userId)
        {
            var friends = await _farmService.GetFriendsFarmsAsync(userId);

            return Ok(friends);
        }

        [HttpPost("collaborators")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> InviteCollaborator(Guid userId, [FromBody] UserForInvitingDto userDto)
        {
            await _farmService.InviteFriendAsync(userId, userDto);
            return NoContent();
        }

        [HttpPut]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateFarm(Guid userId, [FromBody] FarmForUpdateDto farmDto)
        {
            await _farmService.UpdateFarmNameAsync(userId, farmDto);

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFarm(Guid userId)
        {
            await _farmService.DeleteFarmById(userId);

            return NoContent();
        }
    }
}
