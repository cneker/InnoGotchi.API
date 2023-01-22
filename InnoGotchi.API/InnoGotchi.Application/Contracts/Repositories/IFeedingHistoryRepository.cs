using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.Contracts.Repositories
{
    public interface IFeedingHistoryRepository
    {
        Task<IEnumerable<FeedingRecord>> GetHistoryByFarmIdAsync(Guid farmId, bool trackChanges);
        Task<FeedingRecord> GetLastFedByPetIdAsync(Guid petId, bool trackChanges);
        Task CreateRecordAsync(FeedingRecord record);
        void DeleteRecord(FeedingRecord record);

    }
}
