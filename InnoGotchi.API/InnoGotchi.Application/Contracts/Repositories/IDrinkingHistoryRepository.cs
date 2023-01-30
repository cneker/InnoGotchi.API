using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.Contracts.Repositories
{
    public interface IDrinkingHistoryRepository
    {
        Task<IEnumerable<ThirstyStateChanges>> GetHistoryByFarmIdAsync(Guid farmId, bool trackChanges);
        Task<ThirstyStateChanges> GetLastDrankByPetIdAsync(Guid petId, bool trackChanges);
        Task CreateRecordAsync(ThirstyStateChanges record);
        void DeleteRecord(ThirstyStateChanges record);
    }
}
