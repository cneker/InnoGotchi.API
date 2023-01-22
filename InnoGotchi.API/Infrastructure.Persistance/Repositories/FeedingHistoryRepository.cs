using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class FeedingHistoryRepository : RepositoryBase<FeedingRecord>, IFeedingHistoryRepository
    {
        public FeedingHistoryRepository(AppDbContext repositoryContext) 
            : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<FeedingRecord>> GetHistoryByFarmIdAsync(Guid farmId, bool trackChanges) =>
            await GetAll(trackChanges)
            .Include(r => r.Pet)
            .Where(r => r.Pet.FarmId == farmId)
            .ToListAsync();

        public async Task<FeedingRecord> GetLastFedByPetIdAsync(Guid petId, bool trackChanges) =>
            await GetByCondition(r => r.PetId == petId, trackChanges).LastOrDefaultAsync();

        public async Task CreateRecordAsync(FeedingRecord record) =>
            await Create(record);

        public void DeleteRecord(FeedingRecord record)
        {
            Delete(record);
        }
    }
}
