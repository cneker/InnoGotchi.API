using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class DrinkingHistoryRepository : RepositoryBase<ThirstyStateChanges>, IDrinkingHistoryRepository
    {
        public DrinkingHistoryRepository(AppDbContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<ThirstyStateChanges>> GetHistoryByFarmIdAsync(Guid farmId, bool trackChanges) =>
            await GetAll(trackChanges)
            .Include(r => r.Pet)
            .Where(r => r.Pet.FarmId == farmId)
            .ToListAsync();

        public async Task<IEnumerable<ThirstyStateChanges>> GetHistoryByPetIdAsync(Guid petId, bool trackChanges) =>
            await GetAll(trackChanges)
            .Where(r => r.PetId == petId)
            .ToListAsync();

        public async Task<ThirstyStateChanges> GetLastDrankByPetIdAsync(Guid petId, bool trackChanges) =>
            await GetByCondition(r => r.PetId == petId, trackChanges).LastOrDefaultAsync();

        public async Task CreateRecordAsync(ThirstyStateChanges record) =>
            await Create(record);

        public void DeleteRecord(ThirstyStateChanges record)
        {
            Delete(record);
        }
    }
}
