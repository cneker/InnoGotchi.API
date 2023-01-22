using InnoGotchi.Application;
using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Domain.Entities;
using InnoGotchi.Domain.Enums;

namespace Infrastructure.Services
{
    public class PetConditionService : IPetConditionService
    {
        private readonly IRepositoryManager _repositoryManager;
        public PetConditionService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
        public bool IsPetAlive(Pet pet) =>
            pet.DeathDay == null;

        public async Task UpdatePetsFeedingAndDrinkingLevelsByFarm(Guid farmId)
        {
            var pets = await _repositoryManager.PetRepository.GetPetsByFarmIdAsync(farmId, true);
            foreach (var pet in pets)
            {
                await UpdatePetFeedingAndDrinkingLevels(pet);
            }
        }

        public async Task<Pet> UpdatePetFeedingAndDrinkingLevels(Pet pet)
        {
            var lastFed = pet.FeedingRecords.Last();
            var lastDrank = pet.DrinkingRecords.Last();
            var hungryDuration = (DateTime.Now - lastFed.FeedingDate).TotalHours;
            var thirstyDuration = (DateTime.Now - lastDrank.DringkingDate).TotalHours;
            
            while(hungryDuration >= PetConfiguration.FeedingFrequencyInHours)
            {
                hungryDuration -= PetConfiguration.FeedingFrequencyInHours;
                pet.HungerLevel -= 1;
                if(pet.HungerLevel == HungerLevel.Dead)
                {
                    pet.DeathDay = DateTime.Now;
                    break;
                }
            }
            while (thirstyDuration >= PetConfiguration.DrinkingFrequencyInHours)
            {
                thirstyDuration -= PetConfiguration.DrinkingFrequencyInHours;
                pet.ThirstyLevel -= 1;
                if (pet.ThirstyLevel == ThirstyLevel.Dead)
                {
                    pet.DeathDay = DateTime.Now;
                    break;
                }
            }
            await _repositoryManager.SaveAsync();

            return pet;
        }
        
        public int CalculateAge(Pet pet) =>
            pet.DeathDay != null ?
                (pet.DeathDay - pet.Birthday).Value.Days / PetConfiguration.OnePetAgeInDays :
                (DateTime.Now - pet.Birthday).Days / PetConfiguration.OnePetAgeInDays;
    }
}
