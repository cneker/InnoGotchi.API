using InnoGotchi.API.Filters.ActionFilters;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Pet;
using InnoGotchi.Application.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoGotchi.API.Controllers
{
    [Route("api/users/{userId}/farm/{farmId}/pets")]
    [ApiController]
    public class PetsController : ControllerBase
    {
        private readonly IPetService _petService;
        private readonly ILogger _logger;
        public PetsController(IPetService petService, ILogger<PetsController> logger)
        {
            _petService = petService;
            _logger = logger;
        }

        [HttpGet("/api/pets")]
        public async Task<IActionResult> GetPets([FromQuery] PetParameters petParameters)
        {
            var pets = await _petService.GetAllPetsAsync(petParameters);
            _logger.LogInformation("Send all pets");
            return Ok(pets);
        }

        [HttpGet("{id}", Name = "GetPet"), Authorize]
        public async Task<IActionResult> GetPet(Guid userId, Guid farmId, Guid id)
        {
            var pet = await _petService.GetPetByIdAsync(userId, farmId, id);
            _logger.LogInformation("Send pet");
            return Ok(pet);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute)), Authorize]
        public async Task<IActionResult> CreatePet(Guid userId, Guid farmId, [FromBody] PetForCreationDto petDto)
        {
            var pet = await _petService.CreatePetAsync(userId, farmId, petDto);
            _logger.LogInformation("Pet was created");
            return CreatedAtRoute("GetPet", new { userId, farmId, id = pet.Id }, pet);
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute)), Authorize]
        public async Task<IActionResult> UpdatePetName(Guid userId, Guid farmId, Guid id, [FromBody] PetForUpdateDto petDto)
        {
            await _petService.UpdatePetNameAsync(userId, farmId, id, petDto);
            _logger.LogInformation("Pet was updated");
            return NoContent();
        }

        [HttpPut("{id}/feed"), Authorize]
        public async Task<IActionResult> FeedThePet(Guid userId, Guid farmId, Guid id)
        {
            await _petService.FeedThePetAsync(userId, farmId, id);
            _logger.LogInformation("Pet was feeded");
            return NoContent();
        }

        [HttpPut("{id}/give-a-drink"), Authorize]
        public async Task<IActionResult> GiveADrinkToThePet(Guid userId, Guid farmId, Guid id)
        {
            await _petService.GiveADrinkToPetAsync (userId, farmId, id);
            _logger.LogInformation("Given drink to the pet");
            return NoContent();
        }
    }
}
