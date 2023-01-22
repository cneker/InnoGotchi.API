using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class DrinkingHistoryRepository : RepositoryBase<DrinkingRecord>, IDrinkingHistoryRepository
    {
        public DrinkingHistoryRepository(AppDbContext repositoryContext) 
            : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<DrinkingRecord>> GetHistoryByFarmIdAsync(Guid farmId, bool trackChanges) =>
            await GetAll(trackChanges)
            .Include(r => r.Pet)
            .Where(r => r.Pet.FarmId == farmId)
            .ToListAsync();

        public async Task<DrinkingRecord> GetLastDrankByPetIdAsync(Guid petId, bool trackChanges) =>
            await GetByCondition(r => r.PetId == petId, trackChanges).LastOrDefaultAsync();

        public async Task CreateRecordAsync(DrinkingRecord record) =>
            await Create(record);

        public void DeleteRecord(DrinkingRecord record)
        {
            Delete(record);
        }
    }
}
