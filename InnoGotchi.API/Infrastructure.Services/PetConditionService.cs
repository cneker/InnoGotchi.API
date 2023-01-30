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

        public async Task UpdatePetsFeedingAndDrinkingLevelsByFarm(Farm farm)
        {
            var pets = await _repositoryManager.PetRepository.GetPetsByFarmIdAsync(farm.Id, true);
            foreach (var pet in farm.Pets)
            {
                await UpdatePetFeedingAndDrinkingLevels(pet);
            }
            farm.Pets = pets.ToList();
        }

        public async Task<Pet> UpdatePetFeedingAndDrinkingLevels(Pet pet)
        {
            if (!IsPetAlive(pet))
                return pet;

            var lastHungryUpdate = pet.HungryStateChangesHistory
                .OrderBy(f => f.ChangesDate).Last();
            var lastThirstyUpdate = pet.ThirstyStateChangesHistory
                .OrderBy(d => d.ChangesDate).Last();
            var hungryDuration = (DateTime.Now - lastHungryUpdate.ChangesDate).TotalHours;
            var thirstyDuration = (DateTime.Now - lastThirstyUpdate.ChangesDate).TotalHours;
            
            while(hungryDuration >= PetConfiguration.FeedingFrequencyInHours)
            {
                hungryDuration -= PetConfiguration.FeedingFrequencyInHours;
                pet.HungerLevel -= 1;

                var feeding = new HungryStateChanges() { PetId = pet.Id, ChangesDate = DateTime.Now };
                await _repositoryManager.FeedingHistoryRepository.CreateRecordAsync(feeding);

                if (pet.HungerLevel == HungerLevel.Dead)
                {
                    pet.DeathDay = DateTime.Now;
                    break;
                }
            }
            while (thirstyDuration >= PetConfiguration.DrinkingFrequencyInHours)
            {
                thirstyDuration -= PetConfiguration.DrinkingFrequencyInHours;
                pet.ThirstyLevel -= 1;

                var drinking = new ThirstyStateChanges() { PetId = pet.Id, ChangesDate = DateTime.Now };
                await _repositoryManager.DrinkingHistoryRepository.CreateRecordAsync(drinking);

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
