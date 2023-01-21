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
            farm.AverageFeedingPeriod =
                (await _repositoryManager.PetRepository.GetDeadPetsByFarmAsync(farm.Id, false))
                .Average(p => p.FeedingPeriod);
            farm.AverageThirstQuenchingPeriod =
                (await _repositoryManager.PetRepository.GetDeadPetsByFarmAsync(farm.Id, false))
                .Average(p => p.ThirstQuenchingPeriod);
            return farm;
        }
    }
}
