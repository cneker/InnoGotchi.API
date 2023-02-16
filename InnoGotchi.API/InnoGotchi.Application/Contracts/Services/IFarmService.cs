﻿using InnoGotchi.Application.DataTransferObjects.Farm;
using InnoGotchi.Application.DataTransferObjects.User;

namespace InnoGotchi.Application.Contracts.Services
{
    public interface IFarmService
    {
        Task<FarmOverviewDto> CreateFarmAsync(Guid userId, FarmForCreationDto farmForCreation);
        Task<IEnumerable<FarmOverviewDto>> GetFarmsOverviewAsync();
        Task<IEnumerable<FarmOverviewDto>> GetFriendsFarmsAsync(Guid userId);
        Task<FarmOverviewDto> GetFarmOverviewByIdAsync(Guid userId);
        Task<FarmDetailsDto> GetFarmDetailsByFarmIdAsync(Guid userId, Guid farmId);
        Task<FarmStatisticsDto> GetFarmStatisticsByFarmIdAsync(Guid userId, Guid farmId);
        Task UpdateFarmNameAsync(Guid userId, FarmForUpdateDto farmForUpdate);
        Task InviteFriendAsync(Guid userId, UserForInvitingDto userForInviting);
        Task DeleteFarmById(Guid id);
    }
}
