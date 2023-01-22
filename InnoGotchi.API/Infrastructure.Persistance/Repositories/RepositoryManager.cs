using InnoGotchi.Application.Contracts.Repositories;

namespace Infrastructure.Persistance.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private AppDbContext _repositoryContext;
        private IUserRepository _userRepository;
        private IFarmRepository _farmRepository;
        private IPetRepository _petRepository;
        private IFeedingHistoryRepository _feedingHistoryRepository;
        private IDrinkingHistoryRepository _drinkingHistoryRepository;

        public RepositoryManager(AppDbContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }
        public IUserRepository UserRepository
        {
            get
            {
                if (_userRepository == null)
                    _userRepository = new UserRepository(_repositoryContext);
                return _userRepository;
            }
        }

        public IFarmRepository FarmRepository
        {
            get
            {
                if (_farmRepository == null)
                    _farmRepository = new FarmRepository(_repositoryContext);
                return _farmRepository;
            }
        }

        public IPetRepository PetRepository
        {
            get
            {
                if (_petRepository == null)
                    _petRepository = new PetRepository(_repositoryContext);
                return _petRepository;
            }
        }

        public IFeedingHistoryRepository FeedingHistoryRepository
        {
            get
            {
                if (_feedingHistoryRepository == null)
                    _feedingHistoryRepository = new FeedingHistoryRepository(_repositoryContext);
                return _feedingHistoryRepository;
            }
        }

        public IDrinkingHistoryRepository DrinkingHistoryRepository
        {
            get
            {
                if (_drinkingHistoryRepository == null)
                    _drinkingHistoryRepository = new DrinkingHistoryRepository(_repositoryContext);
                return _drinkingHistoryRepository;
            }
        }

        public async Task SaveAsync() =>
            await _repositoryContext.SaveChangesAsync();
    }
}
