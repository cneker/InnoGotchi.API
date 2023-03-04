using InnoGotchi.API.Filters.ActionFilters;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Pet;
using InnoGotchi.Application.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace InnoGotchi.API.Controllers
{
    [Route("api/users/{userId}/farm/{farmId}/pets")]
    [Consumes("application/json")]
    [Produces("application/json")]
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

        /// <summary>
        /// Get a list of all pets
        /// </summary>
        /// <param name="petParameters">Query parameters for sorting, count of items to get</param>
        /// <returns>A pets list</returns>
        /// <response code="200">Returns a pets list</response>
        [HttpGet("/api/pets")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPets([FromQuery] PetParameters petParameters)
        {
            var pets = await _petService.GetAllPetsAsync(petParameters);

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pets.MetaData));

            _logger.LogInformation("Send all pets");
            return Ok(pets.PetOverviewDtos);
        }

        /// <summary>
        /// Get a pet
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="farmId">Farm Id</param>
        /// <param name="petId">Pet Id</param>
        /// <returns>An existing pet item</returns>
        /// <response code="200">Returns an existing pet item</response>
        /// <responce code="404">If the pet Id or the farm Id are invalid</responce>
        /// <response code="403">If the user isn't the owner or the collaborator of the farm or if you try to make an action as another user</response>
        [HttpGet("{petId}", Name = "GetPet"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> GetPet(Guid userId, Guid farmId, Guid petId)
        {
            var pet = await _petService.GetPetByIdAsync(userId, farmId, petId);
            _logger.LogInformation("Send pet");
            return Ok(pet);
        }

        /// <summary>
        /// Create a new pet
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="farmId">Farm Id</param>
        /// <param name="petDto">Model for creating a pet</param>
        /// <returns>A newly created pet</returns>
        /// <response code="200">Retuns a newly created pet</response>
        /// <responce code="404">If the farm Id is invalid</responce>
        /// <response code="403">If the user isn't the owner of the farm or if you try to make an action as another user</response>
        /// <response code="400">If passed pet name has alredy registered</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(CheckUserIdAttribute)), Authorize]
        public async Task<IActionResult> CreatePet(Guid userId, Guid farmId, [FromBody] PetForCreationDto petDto)
        {
            var pet = await _petService.CreatePetAsync(userId, farmId, petDto);
            _logger.LogInformation("Pet was created");
            return CreatedAtRoute("GetPet", new { userId, farmId, petId = pet.Id }, pet);
        }

        /// <summary>
        /// Update information about a pet
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="farmId">Farm Id</param>
        /// <param name="petId">Pet Id</param>
        /// <param name="petDto">Model with updated information</param>
        /// <returns>Nothing</returns>
        /// <response code="204">If information is successfully updated</response>
        /// <responce code="404">If the farm Id or the pet Id are invalid</responce>
        /// <response code="403">If the user isn't the owner of the farm or if you try to make an action as another user</response>
        /// <response code="400">If passed pet name has alredy registered</response>
        [HttpPut("{petId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(CheckUserIdAttribute)), Authorize]
        public async Task<IActionResult> UpdatePetName(Guid userId, Guid farmId, Guid petId, [FromBody] PetForUpdateDto petDto)
        {
            await _petService.UpdatePetNameAsync(userId, farmId, petId, petDto);
            _logger.LogInformation("Pet was updated");
            return NoContent();
        }

        /// <summary>
        /// Feed the pet
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="farmId">Farm Id</param>
        /// <param name="petId">Pet Id</param>
        /// <returns>Nothing</returns>        
        /// <response code="204">If the pet is successfully feeded</response>
        /// <responce code="404">If the farm Id or the pet Id are invalid</responce>
        /// <response code="403">If the user isn't the owner or the collaborator of the farm or if you try to make an action as another user</response>
        /// <response code="423">If the pet is dead</response>
        [HttpPut("{petId}/feed"), Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> FeedThePet(Guid userId, Guid farmId, Guid petId)
        {
            await _petService.FeedThePetAsync(userId, farmId, petId);
            _logger.LogInformation("Pet was feeded");
            return NoContent();
        }

        /// <summary>
        /// Give a drink to the pet
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="farmId">Farm Id</param>
        /// <param name="petId">Pet Id</param>
        /// <returns>Nothing</returns>        
        /// <response code="204">If the pet is successfully watered</response>
        /// <responce code="404">If the farm Id or the pet Id are invalid</responce>
        /// <response code="403">If the user isn't the owner or the collaborator of the farm or if you try to make an action as another user</response>
        /// <response code="423">If the pet is dead</response>
        [HttpPut("{petId}/give-a-drink"), Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        [ServiceFilter(typeof(CheckUserIdAttribute))]
        public async Task<IActionResult> GiveADrinkToThePet(Guid userId, Guid farmId, Guid petId)
        {
            await _petService.GiveADrinkToPetAsync(userId, farmId, petId);
            _logger.LogInformation("Given drink to the pet");
            return NoContent();
        }
    }
}
