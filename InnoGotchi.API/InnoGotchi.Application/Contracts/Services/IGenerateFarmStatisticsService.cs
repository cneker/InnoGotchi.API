using InnoGotchi.Application.DataTransferObjects.Farm;

namespace InnoGotchi.Application.Contracts.Services
{
    public interface IGenerateFarmStatisticsService
    {
        Task<FarmStatisticsDto> GenerateStatistics(FarmStatisticsDto farm);
    }
}
