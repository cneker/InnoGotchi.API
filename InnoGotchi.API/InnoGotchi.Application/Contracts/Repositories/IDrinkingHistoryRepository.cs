using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.Contracts.Repositories
{
    public interface IDrinkingHistoryRepository
    {
        Task<IEnumerable<DrinkingRecord>> GetHistoryByFarmIdAsync(Guid farmId, bool trackChanges);
        Task<DrinkingRecord> GetLastDrankByPetIdAsync(Guid petId, bool trackChanges);
        Task CreateRecordAsync(DrinkingRecord record);
        void DeleteRecord(DrinkingRecord record);
    }
}
