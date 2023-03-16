using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.Contracts.Repositories
{
    public interface IFarmRepository
    {
        Task<IEnumerable<Farm>> GetFarmsAsync(bool trackChanges);
        Task<Farm> GetFarmByIdAsync(Guid id, bool trackChanges);
        Task<Farm> GetFarmByUserIdAsync(Guid userId, bool trackChanges);
        Task<Farm> GetFarmByNameAsync(string name, bool trackChanges);
        Task CreateFarmAsyncs(Farm farm);
        void DeleteFarm(Farm farm);
    }
}