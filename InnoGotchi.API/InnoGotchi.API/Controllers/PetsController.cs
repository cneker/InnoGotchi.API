using InnoGotchi.API.Filters.ActionFilters;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Pet;
using Microsoft.AspNetCore.Mvc;

namespace InnoGotchi.API.Controllers
{
    [Route("api/users/{userId}/farms/{farmId}/pets")]
    [ApiController]
    public class PetsController : ControllerBase
    {
        private readonly IPetService _petService;
        public PetsController(IPetService petService)
        {
            _petService = petService;
        }

        [HttpGet("/api/pets")]
        public async Task<IActionResult> GetPets()
        {
            var pets = await _petService.GetAllPetsAsync();

            return Ok(pets);
        }

        [HttpGet("{id}", Name = "GetPet")]
        public async Task<IActionResult> GetPet(Guid userId, Guid farmId, Guid id)
        {
            var pet = await _petService.GetPetByIdAsync(userId, farmId, id);

            return Ok(pet);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreatePet(Guid userId, Guid farmId, [FromBody] PetForCreationDto petDto)
        {
            var pet = await _petService.CreatePetAsync(userId, farmId, petDto);

            return CreatedAtRoute("GetPet", new { userId, farmId, id = pet.Id }, pet);
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdatePetName(Guid userId, Guid farmId, Guid id, [FromBody] PetForUpdateDto petDto)
        {
            await _petService.UpdatePetNameAsync(userId, farmId, id, petDto);

            return NoContent();
        }

        [HttpPut("{id}/feed")]
        public async Task<IActionResult> FeedThePet(Guid userId, Guid farmId, Guid id)
        {
            await _petService.FeedThePetAsync(userId, farmId, id);

            return NoContent();
        }

        [HttpPut("{id}/give-a-drink")]
        public async Task<IActionResult> GiveADrinkToThePet(Guid userId, Guid farmId, Guid id)
        {
            await _petService.GiveADrinkToPetAsync (userId, farmId, id);

            return NoContent();
        }
    }
}
