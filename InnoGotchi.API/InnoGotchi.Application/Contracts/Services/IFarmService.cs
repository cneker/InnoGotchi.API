using InnoGotchi.Application.DataTransferObjects.Farm;
using InnoGotchi.Application.DataTransferObjects.User;

namespace InnoGotchi.Application.Contracts.Services
{
    public interface IFarmService
    {
        Task<Guid> CreateFarmAsync(Guid userId, FarmForCreationDto farmForCreation);
        Task<IEnumerable<FarmOverviewDto>> GetFarmsOverviewAsync();
        Task<IEnumerable<FarmOverviewDto>> GetFriendsFarmsAsync(Guid userId);
        Task<FarmOverviewDto> GetFarmOverviewByIdAsync(Guid userId);
        Task<FarmDetailsDto> GetFarmDetailsByIdAsync(Guid userId);
        Task<FarmStatisticsDto> GetFarmStatisticsByIdAsync(Guid userId);
        Task UpdateFarmNameAsync(Guid userId, FarmForUpdateDto farmForUpdate);
        Task InviteFriendAsync(Guid userId, UserForInvitingDto userForInviting);
        Task DeleteFarmById(Guid id);
    }
}
