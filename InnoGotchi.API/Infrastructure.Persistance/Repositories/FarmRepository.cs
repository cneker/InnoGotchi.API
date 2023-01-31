using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class FarmRepository : RepositoryBase<Farm>, IFarmRepository
    {
        public FarmRepository(AppDbContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Farm>> GetFarmsAsync(bool trackChanges) =>
            await GetAll(trackChanges)
            .Include(p => p.Pets)
            .ToListAsync();
        //мб лучше по имени, тогда и смысл в нижестоящем методе пропадет
        public async Task<Farm> GetFarmByIdAsync(Guid id, bool trackChanges) =>
            await GetByCondition(f => f.Id == id, trackChanges)
            .Include(f => f.Collaborators)
            .Include(p => p.Pets)
            .SingleOrDefaultAsync();
        
        public async Task<Farm> GetFarmByUserIdAsync(Guid userId, bool trackChanges) =>
            await GetByCondition(f => f.UserId == userId, trackChanges)
            .Include(f => f.Collaborators)
            .Include(p => p.Pets)
            .SingleOrDefaultAsync();

        public async Task<Farm> GetFarmByNameAsync(string name, bool trackChanges) =>
            await GetByCondition(f => f.Name == name, trackChanges)
            .SingleOrDefaultAsync();

        public async Task CreateFarm(Farm farm) =>
            await Create(farm);

        public void DeleteFarm(Farm farm) =>
            Delete(farm);

    }
}
