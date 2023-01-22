using InnoGotchi.Application.DataTransferObjects.Pet;

namespace InnoGotchi.Application.Contracts.Services
{
    public interface IPetService
    {
        Task<IEnumerable<PetOverviewDto>> GetAllPetsAsync();
        Task<PetDetailsDto> GetPetByIdAsync(Guid id);
        Task<Guid> CreatePetAsync(Guid farmId, PetForCreationDto petForCreation);
        Task UpdatePetNameAsync(Guid id, PetForUpdateDto petForUpdate);
        Task FeedThePetAsync(Guid id);
        Task GiveADrinkToPetAsync(Guid id);
    }
}
