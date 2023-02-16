using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(AppDbContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<User>> GetUsersAsync(bool trackChanges) =>
            await GetAll(trackChanges)
            .ToListAsync();

        public async Task<User> GetUserByIdAsync(Guid id, bool trackChanged) =>
            await GetByCondition(u => u.Id == id, trackChanged)
            .Include(u => u.UserFarm)
            .Include(u => u.FriendsFarms)
            .ThenInclude(f => f.Pets)
            .SingleOrDefaultAsync();

        public async Task<User> GetUserByEmailAsync(string email, bool trackChanged) =>
            await GetByCondition(u => u.Email == email, trackChanged)
            .Include(u => u.UserFarm)
            .Include(u => u.FriendsFarms)
            .SingleOrDefaultAsync();

        public async Task CreateUser(User user) =>
            await Create(user);

        public void DeleteUser(User user) =>
            Delete(user);
    }
}
