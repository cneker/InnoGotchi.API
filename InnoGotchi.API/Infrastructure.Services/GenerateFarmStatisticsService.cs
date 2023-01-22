using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Farm;

namespace Infrastructure.Services
{
    public class GenerateFarmStatisticsService : IGenerateFarmStatisticsService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ICalculatePetAgeService _calculateAge;
        public GenerateFarmStatisticsService(IRepositoryManager repositoryManager,
            ICalculatePetAgeService calculateAge)
        {
            _repositoryManager = repositoryManager;
            _calculateAge = calculateAge;
        }

        public async Task<FarmStatisticsDto> GenerateStatistics(FarmStatisticsDto farm)
        {
            farm.AlivePetsCount = 
                (await _repositoryManager.PetRepository.GetAlivePetsByFarmAsync(farm.Id, false)).Count();
            farm.DeadPetsCount =
                (await _repositoryManager.PetRepository.GetDeadPetsByFarmAsync(farm.Id, false)).Count();
            farm.AveragePetsAge =
                Convert.ToInt32(
                    (await _repositoryManager.PetRepository.GetPetsByFarmIdAsync(farm.Id, false))
                    .Average(p => _calculateAge.CalculateAge(p.Birthday, p.DeathDay)));
            farm.AveragePetsHappinessDaysCount =
                Convert.ToInt32(
                    (await _repositoryManager.PetRepository.GetDeadPetsByFarmAsync(farm.Id, false))
                    .Average(p => p.HappynessDayCount));
            farm.AverageFeedingPeriod = await CalculateAverageFeedingPeriod(farm.Id);
            farm.AverageThirstQuenchingPeriod = await CalculateAverageDrinkingPeriod(farm.Id);
            return farm;
        }

        private async Task<double> CalculateAverageFeedingPeriod(Guid farmId)
        {
            var feedingRecords = await _repositoryManager
                .FeedingHistoryRepository
                .GetHistoryByFarmIdAsync(farmId, false);

            var dates = feedingRecords.Select(r => r.FeedingDate).ToArray();
            double period = 0.0;
            for(int i = 1; i < dates.Count(); i++)
            {
                period += (dates[i] - dates[i - 1]).TotalHours;
            }
            return period;
        }

        private async Task<double> CalculateAverageDrinkingPeriod(Guid farmId)
        {
            var feedingRecords = await _repositoryManager
                .DrinkingHistoryRepository
                .GetHistoryByFarmIdAsync(farmId, false);

            var dates = feedingRecords.Select(r => r.DringkingDate).ToArray();
            double period = 0.0;
            for (int i = 1; i < dates.Count(); i++)
            {
                period += (dates[i] - dates[i - 1]).TotalHours;
            }
            return period;
        }
    }
}
