using InnoGotchi.Application.DataTransferObjects.Farm;
using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.Contracts.Services
{
    public interface IGenerateFarmStatisticsService
    {
        Task<FarmStatisticsDto> GenerateStatistics(FarmStatisticsDto farm);
    }
}
