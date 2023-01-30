using InnoGotchi.API.Filters.ActionFilters;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Pet;
using Microsoft.AspNetCore.Mvc;

namespace InnoGotchi.API.Controllers
{
    [Route("api/farms/{farmId}/pets")]
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
        public async Task<IActionResult> GetPet(Guid farmId, Guid id)
        {
            var pet = await _petService.GetPetByIdAsync(farmId, id);

            return Ok(pet);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreatePet(Guid farmId, [FromBody] PetForCreationDto petDto)
        {
            var petId = await _petService.CreatePetAsync(farmId, petDto);

            return CreatedAtRoute("GetPet", new { farmId, id = petId }, petId);
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdatePetName(Guid farmId, Guid id, [FromBody] PetForUpdateDto petDto)
        {
            await _petService.UpdatePetNameAsync(farmId, id, petDto);

            return NoContent();
        }

        [HttpPut("{id}/feed")]
        public async Task<IActionResult> FeedThePet(Guid farmId, Guid id)
        {
            await _petService.FeedThePetAsync(farmId, id);

            return NoContent();
        }

        [HttpPut("{id}/give-a-drink")]
        public async Task<IActionResult> GiveADrinkToThePet(Guid farmId, Guid id)
        {
            await _petService.GiveADrinkToPetAsync(farmId, id);

            return NoContent();
        }
    }
}
