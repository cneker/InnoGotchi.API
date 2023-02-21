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
            var feeding = new HungryStateChanges();
            feeding.ChangesDate = lastHungryUpdate.ChangesDate;
            var drinking = new ThirstyStateChanges();
            drinking.ChangesDate = lastThirstyUpdate.ChangesDate;

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
                    pet.ThirstyLevel = ThirstyLevel.Dead;
                    pet.DeathDay = feeding.ChangesDate;
                    break;
                }
            }
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
                    pet.HungerLevel = HungerLevel.Dead;
                    if (pet.DeathDay > drinking.ChangesDate || pet.DeathDay == null)
                        pet.DeathDay = drinking.ChangesDate;
                    break;
                }
            }

            await _repositoryManager.SaveAsync();

            pet.HappynessDayCount = await CalculateHappynessDayCount(pet.Id);

            await _repositoryManager.SaveAsync();

            return pet;
        }

        public int CalculateAge(Pet pet) =>
            pet.DeathDay != null ?
                (pet.DeathDay - pet.Birthday).Value.Days / PetConfiguration.OnePetAgeInDays :
                (DateTime.Now - pet.Birthday).Days / PetConfiguration.OnePetAgeInDays;

        public async Task<double> CalculateHappynessDayCount(Guid petId)
        {
            var hungryRecords =
                (await _repositoryManager.FeedingHistoryRepository.GetHistoryByPetIdAsync(petId, false)).ToList();
            var thirstyRecords =
                (await _repositoryManager.DrinkingHistoryRepository.GetHistoryByPetIdAsync(petId, false)).ToList();
            hungryRecords.Add(new HungryStateChanges { PetId = petId, ChangesDate = DateTime.Now });
            thirstyRecords.Add(new ThirstyStateChanges { PetId = petId, ChangesDate = DateTime.Now });

            var startH = DateTime.Now; var endH = DateTime.Now;
            var startT = DateTime.Now; var endT = DateTime.Now;

            int i = 0, j = 0;

            var count = 0.0;

            bool emptyH = true; bool emptyT = true;
            bool nextT = true, nextH = true;

            while (true)
            {
                if (nextH)
                {
                    for (; i < hungryRecords.Count(); i++)
                    {
                        if (hungryRecords[i].HungerState >= HungerLevel.Normal)
                        {
                            startH = hungryRecords[i].ChangesDate;
                            i++;
                            emptyH = false;
                            break;
                        }
                    }

                    for (; i < hungryRecords.Count(); i++)
                    {
                        if (hungryRecords[i].HungerState < HungerLevel.Normal)
                        {
                            endH = hungryRecords[i].ChangesDate;
                            i++;
                            break;
                        }
                    }
                }

                if (nextT)
                {
                    for (; j < thirstyRecords.Count(); j++)
                    {
                        if (thirstyRecords[j].ThirstyState >= ThirstyLevel.Normal)
                        {
                            startT = thirstyRecords[j].ChangesDate;
                            j++;
                            emptyT = false;
                            break;
                        }
                    }

                    for (; j < thirstyRecords.Count(); j++)
                    {
                        if (thirstyRecords[j].ThirstyState < ThirstyLevel.Normal)
                        {
                            endT = thirstyRecords[j].ChangesDate;
                            j++;
                            break;
                        }
                    }
                }

                if (emptyH == true || emptyT == true)
                    break;


                if (startH < endT && startT < endH)
                {
                    var temp_start = startH > startT ? startH : startT;
                    var temp_end = endH > endT ? endT : endH;

                    count += (temp_end - temp_start).TotalHours;

                    if (temp_end == endT && temp_end == endH)
                    {
                        break;
                    }

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
                    if (startH > endT)
                    {
                        nextH = false;
                        nextT = true;
                    }
                    else
                    {
                        nextT = false;
                        nextH = true;
                    }
                }
            }
            return count;
        }
    }
}
