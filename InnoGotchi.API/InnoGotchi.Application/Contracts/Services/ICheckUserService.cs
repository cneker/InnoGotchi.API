using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.Contracts.Services
{
    public interface ICheckUserService
    {
        bool CheckWhetherUserIsCollaborator(Farm farm, Guid userId);
        bool CheckWhetherUserIsOwner(Farm farm, Guid userId);
    }
}
