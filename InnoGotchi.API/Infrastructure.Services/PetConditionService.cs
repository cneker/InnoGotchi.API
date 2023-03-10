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

        public async Task UpdatePetsFeedingAndDrinkingLevelsByFarmAsync(Farm farm)
        {
            var pets = await _repositoryManager.PetRepository.GetPetsByFarmIdAsync(farm.Id, true);
            foreach (var pet in farm.Pets)
            {
                await UpdatePetFeedingAndDrinkingLevelsAsync(pet);
            }
            farm.Pets = pets.ToList();
        }

        public async Task<Pet> UpdatePetFeedingAndDrinkingLevelsAsync(Pet pet)
        {
            if (!IsPetAlive(pet))
                return pet;

            await UpdateHungryStateAsync(pet);
            await UpdateThirstyStateAsync(pet);

            if (pet.HungerLevel == HungerLevel.Dead && pet.ThirstyLevel != ThirstyLevel.Dead)
                pet.ThirstyLevel = ThirstyLevel.Dead;
            if (pet.ThirstyLevel == ThirstyLevel.Dead && pet.HungerLevel != HungerLevel.Dead)
                pet.HungerLevel = HungerLevel.Dead;

            pet.HappinessDayCount = CalculateHappynessDayCount(pet);

            await _repositoryManager.SaveAsync();

            return pet;
        }

        private async Task UpdateHungryStateAsync(Pet pet)
        {
            var lastHungryUpdate = pet.HungryStateChangesHistory.OrderBy(f => f.ChangesDate)
                .Last();
            var hungryDuration = (DateTime.Now - lastHungryUpdate.ChangesDate).TotalHours;
            var feeding = new HungryStateChanges
            {
                ChangesDate = lastHungryUpdate.ChangesDate
            };

            while (hungryDuration >= PetConfiguration.FeedingFrequencyInHours)
            {
                hungryDuration -= PetConfiguration.FeedingFrequencyInHours;
                pet.HungerLevel -= 1;

                feeding = new HungryStateChanges()
                {
                    PetId = pet.Id,
                    ChangesDate = feeding.ChangesDate.AddHours(PetConfiguration.FeedingFrequencyInHours),
                    HungerState = pet.HungerLevel
                };
                await _repositoryManager.FeedingHistoryRepository.CreateRecordAsync(feeding);

                if (pet.HungerLevel == HungerLevel.Dead)
                {
                    pet.DeathDay = feeding.ChangesDate;
                    break;
                }
            }
        }

        private async Task UpdateThirstyStateAsync(Pet pet)
        {
            var lastThirstyUpdate = pet.ThirstyStateChangesHistory.OrderBy(d => d.ChangesDate)
                .Last();
            var thirstyDuration = (DateTime.Now - lastThirstyUpdate.ChangesDate).TotalHours;
            var drinking = new ThirstyStateChanges
            {
                ChangesDate = lastThirstyUpdate.ChangesDate
            };

            while (thirstyDuration >= PetConfiguration.DrinkingFrequencyInHours)
            {
                thirstyDuration -= PetConfiguration.DrinkingFrequencyInHours;
                pet.ThirstyLevel -= 1;

                drinking = new ThirstyStateChanges()
                {
                    PetId = pet.Id,
                    ChangesDate = drinking.ChangesDate.AddHours(PetConfiguration.DrinkingFrequencyInHours),
                    ThirstyState = pet.ThirstyLevel
                };
                await _repositoryManager.DrinkingHistoryRepository.CreateRecordAsync(drinking);

                if (pet.ThirstyLevel == ThirstyLevel.Dead)
                {
                    if (pet.DeathDay > drinking.ChangesDate || pet.DeathDay == null)
                        pet.DeathDay = drinking.ChangesDate;
                    break;
                }
            }
        }

        public int CalculateAge(Pet pet) =>
            pet.DeathDay != null ?
                (pet.DeathDay - pet.Birthday).Value.Days / PetConfiguration.OnePetAgeInDays :
                (DateTime.Now - pet.Birthday).Days / PetConfiguration.OnePetAgeInDays;

        public double CalculateHappynessDayCount(Pet pet)
        {
            var hungryRecords = pet.HungryStateChangesHistory.ToList();
            var thirstyRecords = pet.ThirstyStateChangesHistory.ToList();
            var changesDate = DateTime.Now;
            hungryRecords.Add(new HungryStateChanges { PetId = pet.Id, ChangesDate = changesDate });
            thirstyRecords.Add(new ThirstyStateChanges { PetId = pet.Id, ChangesDate = changesDate });

            DateTime startH = DateTime.Now, endH = DateTime.Now;
            DateTime startT = DateTime.Now, endT = DateTime.Now;

            int i = 0, j = 0;

            var count = 0.0;

            bool emptyH = true, emptyT = true;
            bool nextT = true, nextH = true;

            while (true)
            {
                if (nextH)
                {
                    CalculateStartHungryHappyPeriod(hungryRecords, ref i, ref emptyH, ref startH);

                    CalculateEndHungryHappyPeriod(hungryRecords, ref i, ref endH);
                }

                if (nextT)
                {
                    CalculateStartThirstyHappyPeriod(thirstyRecords, ref j, ref emptyT, ref startT);

                    CalculateEndThirstyHappyPeriod(thirstyRecords, ref j, ref endT);
                }

                if (emptyH == true || emptyT == true)
                    break;

                if (startH < endT && startT < endH)
                {
                    var temp_start = startH > startT ? startH : startT;
                    var temp_end = endH > endT ? endT : endH;

                    count += (temp_end - temp_start).TotalHours;

                    if (temp_end.Equals(endT) && temp_end == endH)
                        break;

                    if (temp_end == endT)
                    {
                        startH = temp_end;
                        nextH = false;
                        nextT = true;
                        emptyT = true;
                    }
                    else
                    {
                        startT = temp_end;
                        nextT = false;
                        nextH = true;
                        emptyH = true;
                    }
                }
                else
                {
                    nextH = startH < endT;
                    nextT = startH > endT;
                }
            }
            return Math.Round(count / 24, 1);
        }

        private void CalculateStartHungryHappyPeriod(List<HungryStateChanges> hungryRecords, ref int i, ref bool emptyH, ref DateTime startH)
        {
            foreach (var rec in hungryRecords.Skip(i))
            {
                if (rec.HungerState >= HungerLevel.Normal)
                {
                    i++;
                    emptyH = false;
                    startH = rec.ChangesDate;
                    break;
                }
            }
        }

        private void CalculateEndHungryHappyPeriod(List<HungryStateChanges> hungryRecords, ref int i, ref DateTime endH)
        {
            foreach (var rec in hungryRecords.Skip(i))
            {
                if (rec.HungerState < HungerLevel.Normal)
                {
                    i++;
                    endH = rec.ChangesDate;
                    break;
                }
            }
        }

        private void CalculateStartThirstyHappyPeriod(List<ThirstyStateChanges> thirstyRecords, ref int j, ref bool emptyT, ref DateTime startT)
        {
            foreach (var rec in thirstyRecords.Skip(j))
            {
                if (rec.ThirstyState >= ThirstyLevel.Normal)
                {
                    startT = rec.ChangesDate;
                    j++;
                    emptyT = false;
                    break;
                }
            }
        }

        private void CalculateEndThirstyHappyPeriod(List<ThirstyStateChanges> thirstyRecords, ref int j, ref DateTime endT)
        {
            foreach (var rec in thirstyRecords.Skip(j))
            {
                if (rec.ThirstyState < ThirstyLevel.Normal)
                {
                    endT = rec.ChangesDate;
                    j++;
                    break;
                }
            }
        }
    }
}
