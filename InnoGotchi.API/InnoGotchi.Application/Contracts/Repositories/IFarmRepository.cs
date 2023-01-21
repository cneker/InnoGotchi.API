using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.Contracts.Repositories
{
    public interface IFarmRepository
    {
        Task<IEnumerable<Farm>> GetFarmsAsync(bool trackChanges);
        Task<Farm> GetFarmByIdAsync(Guid id, bool trackChanges);
        Task<Farm> GetFarmByUserIdAsync(Guid userId, bool trackChanges);
        Task CreateFarm(Farm farm);
        void DeleteFarm(Farm farm);
    }
}