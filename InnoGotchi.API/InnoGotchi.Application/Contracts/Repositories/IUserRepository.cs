using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.Contracts.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsersAsync(bool trackChanges);
        Task<User> GetUserByIdAsync(Guid id, bool trackChanges);
        Task<User> GetUserByEmailAsync(string email, bool trackChanges);
        Task CreateUserAsync(User user);
        void DeleteUser(User user);
    }
}
