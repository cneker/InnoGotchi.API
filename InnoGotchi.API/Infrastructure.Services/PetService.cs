using AutoMapper;
using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Pet;
using InnoGotchi.Application.Exceptions;
using InnoGotchi.Domain.Entities;
using InnoGotchi.Domain.Enums;

namespace Infrastructure.Services
{
    public class PetService : IPetService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly IPetConditionService _petConditionService;

        public PetService(IRepositoryManager repositoryManager, IMapper mapper, 
            IPetConditionService petConditionService)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _petConditionService = petConditionService;
        }

        public async Task<Guid> CreatePetAsync(Guid farmId, PetForCreationDto petForCreation)
        {
            await CheckFarmExists(farmId);

            var pet = _mapper.Map<Pet>(petForCreation);
            pet.FarmId = farmId;
            await _repositoryManager.PetRepository.CreatePetAsync(pet);

            var feeding = new HungryStateChanges() { PetId = pet.Id, ChangesDate = DateTime.Now };
            var drinking = new ThirstyStateChanges() { PetId = pet.Id, ChangesDate = DateTime.Now };
            await _repositoryManager.FeedingHistoryRepository.CreateRecordAsync(feeding);
            await _repositoryManager.DrinkingHistoryRepository.CreateRecordAsync(drinking);

            await _repositoryManager.SaveAsync();

            return pet.Id;
        }

        public async Task FeedThePetAsync(Guid farmId, Guid id)
        {
            await CheckFarmExists(farmId);

            //добавить проверку на свой-чужой
            var pet = await _repositoryManager.PetRepository.GetPetByIdAsync(id, true);
            if (pet == null)
                throw new NotFoundException("Pet not found");

            if(!_petConditionService.IsPetAlive(pet))
                throw new PetIsDeadException("Rest and peace");

            var feeding = new HungryStateChanges() 
                { PetId = id, ChangesDate = DateTime.Now, IsFeeding = true };
            await _repositoryManager.FeedingHistoryRepository.CreateRecordAsync(feeding);

            pet.HungerLevel = HungerLevel.Full;
            await _repositoryManager.SaveAsync();
        }

        public async Task GiveADrinkToPetAsync(Guid farmId, Guid id)
        {
            await CheckFarmExists(farmId);

            //добавить проверку на свой-чужой
            var pet = await _repositoryManager.PetRepository.GetPetByIdAsync(id, true);
            if (pet == null)
                throw new NotFoundException("Pet not found");

            if (!_petConditionService.IsPetAlive(pet))
                throw new PetIsDeadException("Rest and peace");

            var drinking = new ThirstyStateChanges() 
                { PetId = id, ChangesDate = DateTime.Now, IsDrinking = true };
            await _repositoryManager.DrinkingHistoryRepository.CreateRecordAsync(drinking);

            pet.ThirstyLevel = ThirstyLevel.Full;
            await _repositoryManager.SaveAsync();
        }

        public async Task<IEnumerable<PetOverviewDto>> GetAllPetsAsync()
        {
            var pets = await _repositoryManager.PetRepository.GetAllPetsAsync(true);
            //UPDATE VITAL SIGNS AND SAVE
            foreach (var pet in pets)
                await _petConditionService.UpdatePetFeedingAndDrinkingLevels(pet);

            return _mapper.Map<IEnumerable<PetOverviewDto>>(pets);
        }

        public async Task<PetDetailsDto> GetPetByIdAsync(Guid farmId, Guid id)
        {
            await CheckFarmExists(farmId);
            var pet = await _repositoryManager.PetRepository.GetPetByIdAsync(id, true);
            if (pet == null)
                throw new NotFoundException("Pet not found");
            //UPDATE VITAL SIGNS AND SAVE
            await _petConditionService.UpdatePetFeedingAndDrinkingLevels(pet);

            var petForReturn = _mapper.Map<PetDetailsDto>(pet);

            return petForReturn;
        }

        public async Task UpdatePetNameAsync(Guid farmId, Guid id, PetForUpdateDto petForUpdate)
        {
            await CheckFarmExists(farmId);

            var pet = await _repositoryManager.PetRepository.GetPetByIdAsync(id, true);
            if (pet == null)
                throw new NotFoundException("Pet not found");

            _mapper.Map(petForUpdate, pet);
            await _repositoryManager.SaveAsync();
        }

        private async Task CheckFarmExists(Guid farmId)
        {
            var farm = await _repositoryManager.FarmRepository.GetFarmByIdAsync(farmId, false);
            if (farm == null)
                throw new NotFoundException("Farm not found");
        }
    }
}
