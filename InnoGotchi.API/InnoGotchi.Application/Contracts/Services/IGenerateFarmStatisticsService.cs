using InnoGotchi.Application.DataTransferObjects.Farm;

namespace InnoGotchi.Application.Contracts.Services
{
    public interface IGenerateFarmStatisticsService
    {
        Task<FarmStatisticsDto> GenerateStatisticsAsync(FarmStatisticsDto farm);
    }
}
