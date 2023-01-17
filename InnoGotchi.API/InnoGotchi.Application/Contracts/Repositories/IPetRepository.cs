using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.Contracts.Repositories
{
    public interface IPetRepository
    {
        Task<IEnumerable<Pet>> GetPetsAsync(bool trackChanges);
        Task<Pet> GetPetByIdAsync(Guid id, bool trackChanges);
        Task CreatePet(Pet pet);
        void DeletePet(Pet pet);
    }
}
