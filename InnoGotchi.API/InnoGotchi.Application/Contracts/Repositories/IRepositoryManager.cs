namespace InnoGotchi.Application.Contracts.Repositories
{
    public interface IRepositoryManager
    {
        IUserRepository UserRepository { get; }
        IFarmRepository FarmRepository { get; }
        IPetRepository PetRepository { get; }
        Task SaveAsync();
    }
}
