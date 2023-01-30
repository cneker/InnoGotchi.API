using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Farm;

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

            farm.AlivePetsCount = 
                (await _repositoryManager.PetRepository.GetAlivePetsByFarmAsync(farm.Id, false)).Count();
            farm.DeadPetsCount =
                (await _repositoryManager.PetRepository.GetDeadPetsByFarmAsync(farm.Id, false)).Count();
            if(pets.Any())
            {
                farm.AveragePetsAge = Convert.ToInt32(pets.Average(p => _petConditionService.CalculateAge(p)));
                farm.AveragePetsHappinessDaysCount = Convert.ToInt32(pets.Average(p => p.HappynessDayCount));
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
                .Select(r => r.ChangesDate).ToArray();
            double period = 0.0;
            for(int i = 1; i < dates.Count(); i++)
            {
                period += (dates[i] - dates[i - 1]).TotalHours;
            }
            return period / (dates.Count() - 1);
        }

        private async Task<double> CalculateAverageDrinkingPeriod(Guid farmId)
        {
            var thirstyStatesChangesRecords = await _repositoryManager
                .DrinkingHistoryRepository
                .GetHistoryByFarmIdAsync(farmId, false);

            var dates = thirstyStatesChangesRecords.Where(r => r.IsDrinking).OrderBy(r => r.ChangesDate)
                .Select(r => r.ChangesDate).ToArray();
            double period = 0.0;
            for (int i = 1; i < dates.Count(); i++)
            {
                period += (dates[i] - dates[i - 1]).TotalHours;
            }
            return period / (dates.Count() - 1);
        }
    }
}
