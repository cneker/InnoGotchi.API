using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class PetRepository : RepositoryBase<Pet>, IPetRepository
    {
        public PetRepository(AppDbContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Pet>> GetPetsAsync(bool trackChanges) =>
            await GetAll(trackChanges)
            .ToListAsync();

        public async Task<Pet> GetPetByIdAsync(Guid id, bool trackChanges) =>
            await GetByCondition(p => p.Id == id, trackChanges)
            .SingleOrDefaultAsync();

        public async Task CreatePet(Pet pet) =>
            await Create(pet);

        public void DeletePet(Pet pet) =>
            Delete(pet);
    }
}
