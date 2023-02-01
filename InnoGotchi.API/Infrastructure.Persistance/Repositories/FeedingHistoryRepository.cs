using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class FeedingHistoryRepository : RepositoryBase<HungryStateChanges>, IFeedingHistoryRepository
    {
        public FeedingHistoryRepository(AppDbContext repositoryContext) 
            : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<HungryStateChanges>> GetHistoryByFarmIdAsync(Guid farmId, bool trackChanges) =>
            await GetAll(trackChanges)
            .Include(r => r.Pet)
            .Where(r => r.Pet.FarmId == farmId)
            .ToListAsync();

        public async Task<IEnumerable<HungryStateChanges>> GetHistoryByPetIdAsync(Guid petId, bool trackChanges) =>
            await GetAll(trackChanges)
            .Where(r => r.PetId == petId)
            .ToListAsync();

        public async Task<HungryStateChanges> GetLastFedByPetIdAsync(Guid petId, bool trackChanges) =>
            await GetByCondition(r => r.PetId == petId, trackChanges).LastOrDefaultAsync();

        public async Task CreateRecordAsync(HungryStateChanges record) =>
            await Create(record);

        public void DeleteRecord(HungryStateChanges record)
        {
            Delete(record);
        }
    }
}
