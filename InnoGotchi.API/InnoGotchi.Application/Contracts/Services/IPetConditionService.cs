using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.Contracts.Services
{
    public interface IPetConditionService
    {
        Task UpdatePetsFeedingAndDrinkingLevelsByFarmAsync(Farm farm);
        Task<Pet> UpdatePetFeedingAndDrinkingLevelsAsync(Pet pet);
        bool IsPetAlive(Pet pet);
        int CalculateAge(Pet pet);
        double CalculateHappynessDayCount(Pet pet);
    }
}
