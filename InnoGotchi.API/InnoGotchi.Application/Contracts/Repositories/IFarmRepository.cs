using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.Contracts.Repositories
{
    public interface IFarmRepository
    {
        Task<IEnumerable<Farm>> GetFarmsAsync(bool trackChanges);
        Task<Farm> GetFarmByIdAsync(Guid id, bool trackChanges);
        Task CreateFarm(Farm farm);
        void DelteFarm(Farm farm);
    }
}
