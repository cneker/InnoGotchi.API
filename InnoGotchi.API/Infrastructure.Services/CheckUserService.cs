using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Domain.Entities;

namespace Infrastructure.Services
{
    public class CheckUserService : ICheckUserService
    {
        public bool CheckWhetherUserIsCollaborator(Farm farm, Guid userId) =>
            farm.Collaborators.FirstOrDefault(c => c.Id == userId) != null;

        public bool CheckWhetherUserIsOwner(Farm farm, Guid userId) =>
            farm.UserId == userId;
    }
}
