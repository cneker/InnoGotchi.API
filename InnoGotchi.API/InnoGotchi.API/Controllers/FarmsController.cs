using InnoGotchi.API.Filters.ActionFilters;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Farm;
using InnoGotchi.Application.DataTransferObjects.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoGotchi.API.Controllers
{
    [Route("api/farms")]
    [ApiController]
    public class FarmsController : ControllerBase
    {
        private readonly IFarmService _farmService;
        public FarmsController(IFarmService farmService)
        {
            _farmService = farmService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFarms()
        {
            var farmsDto = await _farmService.GetFarmsOverviewAsync();

            return Ok(farmsDto);
        }

        [HttpGet("my-farm", Name = "GetFarmOverview")]
        [ServiceFilter(typeof(ExtractUserIdFilterAttribute)), Authorize]
        public async Task<IActionResult> GetFarmOverview(Guid id)
        {
            var farmDto = await _farmService.GetFarmOverviewByIdAsync(id);

            return Ok(farmDto);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute)), Authorize]
        [ServiceFilter(typeof(ExtractUserIdFilterAttribute))]
        public async Task<IActionResult> CreateFarm(Guid id, [FromBody] FarmForCreationDto farmDto)
        {
            var farm = await _farmService.CreateFarmAsync(id, farmDto);

            return CreatedAtRoute("GetFarmOverview", new { id = id }, farm);
        }

        [HttpGet("my-farm/detail")]
        [ServiceFilter(typeof(ExtractUserIdFilterAttribute)), Authorize]
        public async Task<IActionResult> GetFarmDetails(Guid id)
        {
            var farmDto = await _farmService.GetFarmDetailsByIdAsync(id);

            return Ok(farmDto);
        }

        [HttpGet("statistics")]
        [ServiceFilter(typeof(ExtractUserIdFilterAttribute)), Authorize]
        public async Task<IActionResult> GetFarmStatistics(Guid id)
        {
            var farmDto = await _farmService.GetFarmStatisticsByIdAsync(id);

            return Ok(farmDto);
        }

        [HttpGet("friends")]
        [ServiceFilter(typeof(ExtractUserIdFilterAttribute))]
        public async Task<IActionResult> GetFriendsFarms(Guid id)
        {
            var friends = await _farmService.GetFriendsFarmsAsync(id);

            return Ok(friends);
        }

        [HttpPost("collaborators")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ExtractUserIdFilterAttribute)), Authorize]
        public async Task<IActionResult> InviteCollaborator(Guid id, [FromBody] UserForInvitingDto userDto)
        {
            await _farmService.InviteFriendAsync(id, userDto);
            return NoContent();
        }

        [HttpPut]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ExtractUserIdFilterAttribute)), Authorize]
        public async Task<IActionResult> UpdateFarm(Guid id, [FromBody] FarmForUpdateDto farmDto)
        {
            await _farmService.UpdateFarmNameAsync(id, farmDto);

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
