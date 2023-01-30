using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.Contracts.Services
{
    public interface IPetConditionService
    {
        Task UpdatePetsFeedingAndDrinkingLevelsByFarm(Farm farm);
        Task<Pet> UpdatePetFeedingAndDrinkingLevels(Pet pet);
        bool IsPetAlive(Pet pet);
        int CalculateAge(Pet pet);
    }
}
