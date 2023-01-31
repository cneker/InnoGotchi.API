using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.Contracts.Repositories
{
    public interface IPetRepository
    {
        Task<IEnumerable<Pet>> GetAllPetsAsync(bool trackChanges);
        Task<IEnumerable<Pet>> GetPetsByFarmIdAsync(Guid farmId, bool trackChanges);
        Task<Pet> GetPetByIdAsync(Guid id, bool trackChanges);
        Task<Pet> GetPetByNameAsync(string name, bool trackChanges);
        Task<IEnumerable<Pet>> GetAlivePetsByFarmAsync(Guid farmId, bool trackChanges);
        Task<IEnumerable<Pet>> GetDeadPetsByFarmAsync(Guid farmId, bool trackChanges);
        Task CreatePetAsync(Pet pet);
        void DeletePet(Pet pet);
    }
}
