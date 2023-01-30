using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Domain.Entities;
using InnoGotchi.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class PetRepository : RepositoryBase<Pet>, IPetRepository
    {
        public PetRepository(AppDbContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Pet>> GetAllPetsAsync(bool trackChanges) =>
            await GetAll(trackChanges)
            .Include(p => p.HungryStateChangesHistory)
            .Include(p => p.ThirstyStateChangesHistory)
            .ToListAsync();

        public async Task<IEnumerable<Pet>> GetPetsByFarmIdAsync(Guid farmId, bool trackChanges) =>
            await GetByCondition(p => p.FarmId == farmId, trackChanges)
            .Include(p => p.HungryStateChangesHistory)
            .Include(p => p.ThirstyStateChangesHistory)
            .ToListAsync();

        public async Task<Pet> GetPetByIdAsync(Guid id, bool trackChanges) =>
            await GetByCondition(p => p.Id == id, trackChanges)
            .Include(p => p.HungryStateChangesHistory)
            .Include(p => p.ThirstyStateChangesHistory)
            .SingleOrDefaultAsync();

        public async Task CreatePetAsync(Pet pet) =>
            await Create(pet);

        public void DeletePet(Pet pet) =>
            Delete(pet);

        public async Task<IEnumerable<Pet>> GetAlivePetsByFarmAsync(Guid farmId, bool trackChanges) =>
            await GetByCondition(p => p.FarmId == farmId && p.HungerLevel != HungerLevel.Dead 
                && p.ThirstyLevel != ThirstyLevel.Dead, trackChanges)
            .ToListAsync();

        public async Task<IEnumerable<Pet>> GetDeadPetsByFarmAsync(Guid farmId, bool trackChanges) =>
            await GetByCondition(p => p.FarmId == farmId && (p.HungerLevel == HungerLevel.Dead
                || p.ThirstyLevel == ThirstyLevel.Dead), trackChanges)
            .ToListAsync();
    }
}
