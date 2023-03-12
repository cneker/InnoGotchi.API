using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.Contracts.Repositories
{
    public interface IFeedingHistoryRepository
    {
        Task<IEnumerable<HungryStateChanges>> GetHistoryByFarmIdAsync(Guid farmId, bool trackChanges);
        Task<IEnumerable<HungryStateChanges>> GetHistoryByPetIdAsync(Guid petId, bool trackChanges);
        Task<HungryStateChanges> GetLastFedByPetIdAsync(Guid petId, bool trackChanges);
        Task CreateRecordAsync(HungryStateChanges record);
        void DeleteRecord(HungryStateChanges record);

    }
}
