using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Farm;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services
{
    public class GenerateFarmStatisticsService : IGenerateFarmStatisticsService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IPetConditionService _petConditionService;
        public GenerateFarmStatisticsService(IRepositoryManager repositoryManager,
            IPetConditionService petConditionService)
        {
            _repositoryManager = repositoryManager;
            _petConditionService = petConditionService;
        }

        public async Task<FarmStatisticsDto> GenerateStatistics(FarmStatisticsDto farm)
        {
            var pets = await _repositoryManager.PetRepository.GetPetsByFarmIdAsync(farm.Id, false);

            farm.AlivePetsCount = pets.Count(p => p.DeathDay == null);
            farm.DeadPetsCount = pets.Count(p => p.DeathDay != null);
            if (pets.Any())
            {
                farm.AveragePetsAge = Convert.ToInt32(pets.Average(p => _petConditionService.CalculateAge(p)));
                farm.AveragePetsHappinessDaysCount = Math.Round(pets.Average(p => p.HappynessDayCount) / 24, 1);
            }
            farm.AverageFeedingPeriod = await CalculateAverageFeedingPeriod(farm.Id);
            farm.AverageThirstQuenchingPeriod = await CalculateAverageDrinkingPeriod(farm.Id);
            return farm;
        }

        private async Task<double> CalculateAverageFeedingPeriod(Guid farmId)
        {
            var hungryStatesChangesRecords = await _repositoryManager
                .FeedingHistoryRepository
                .GetHistoryByFarmIdAsync(farmId, false);
            var dates = hungryStatesChangesRecords.Where(r => r.IsFeeding).OrderBy(r => r.ChangesDate)
                .GroupBy(r => r.PetId).Select(g => g.ToArray());
            double period = 0.0;
            var periods = new List<double>();
            foreach (var petDates in dates)
            {
                for (int i = 1; i < petDates.Count(); i++)
                {
                    period += (petDates[i].ChangesDate - petDates[i - 1].ChangesDate).TotalHours;
                }
                if (petDates.Count() > 1)
                    periods.Add(period / (petDates.Count() - 1));
            }
            return periods.IsNullOrEmpty() ? 0 : Math.Round(periods.Average() / 24, 1);
        }

        private async Task<double> CalculateAverageDrinkingPeriod(Guid farmId)
        {
            var thirstyStatesChangesRecords = await _repositoryManager
                .DrinkingHistoryRepository
                .GetHistoryByFarmIdAsync(farmId, false);

            var dates = thirstyStatesChangesRecords.Where(r => r.IsDrinking).OrderBy(r => r.ChangesDate)
                .GroupBy(r => r.PetId).Select(g => g.ToArray());
            double period = 0.0;
            var periods = new List<double>();
            foreach (var petDates in dates)
            {
                for (int i = 1; i < petDates.Count(); i++)
                {
                    period += (petDates[i].ChangesDate - petDates[i - 1].ChangesDate).TotalHours;
                }
                if (petDates.Count() > 1)
                    periods.Add(period / (petDates.Count() - 1));
            }
            return periods.IsNullOrEmpty() ? 0 : Math.Round(periods.Average() / 24, 1);
        }
    }
}
