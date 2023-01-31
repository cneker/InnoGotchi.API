using InnoGotchi.Application.DataTransferObjects.Pet;

namespace InnoGotchi.Application.Contracts.Services
{
    public interface IPetService
    {
        Task<IEnumerable<PetOverviewDto>> GetAllPetsAsync();
        Task<PetDetailsDto> GetPetByIdAsync(Guid userId, Guid farmId, Guid id);
        Task<PetOverviewDto> CreatePetAsync(Guid userId, Guid farmId, PetForCreationDto petForCreation);
        Task UpdatePetNameAsync(Guid userId, Guid farmId, Guid id, PetForUpdateDto petForUpdate);
        Task FeedThePetAsync(Guid userId, Guid farmId, Guid id);
        Task GiveADrinkToPetAsync(Guid userId, Guid farmId, Guid id);
    }
}
