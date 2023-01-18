using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {

        public UserRepository(RepositoryContext repositoryContext) 
            : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<User>> GetUsersAsync(bool trackChanges) =>
            await GetAll(trackChanges)
            .ToListAsync();

        public async Task<User> GetUserByIdAsync(Guid id, bool trackChanges) =>
            await GetByCondition(u => u.Id == id, trackChanges)
            .SingleOrDefaultAsync();

        public async Task CreateUser(User user) =>
            await Create(user);

        public void DeleteUser(User user) =>
            Delete(user);
    }
}
