using AutoMapper;
using FluentValidation;
using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Pet;
using InnoGotchi.Domain.Entities;
using InnoGotchi.Domain.Enums;

namespace Infrastructure.Services
{
    public class PetService : IPetService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly IValidator<PetForCreationDto> _createPetValidator;
        private readonly IValidator<PetForUpdateDto> _updatePetValidator;
        private readonly IPetConditionService _petConditionService;

        public PetService(IRepositoryManager repositoryManager, IMapper mapper, 
            IValidator<PetForCreationDto> createPetValidator,
            IValidator<PetForUpdateDto> updatePetValidator,
            IPetConditionService petConditionService)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _createPetValidator = createPetValidator;
            _updatePetValidator = updatePetValidator;
            _petConditionService = petConditionService;
        }

        public async Task<Guid> CreatePetAsync(Guid farmId, PetForCreationDto petForCreation)
        {
            var valResult = await _createPetValidator.ValidateAsync(petForCreation);
            if (!valResult.IsValid)
                throw new Exception("invalid data");

            var pet = _mapper.Map<Pet>(petForCreation);
            pet.FarmId = farmId;
            await _repositoryManager.PetRepository.CreatePetAsync(pet);

            var feeding = new FeedingRecord() { PetId = pet.Id, FeedingDate = DateTime.Now };
            var drinking = new DrinkingRecord() { PetId = pet.Id, DringkingDate = DateTime.Now };
            await _repositoryManager.FeedingHistoryRepository.CreateRecordAsync(feeding);
            await _repositoryManager.DrinkingHistoryRepository.CreateRecordAsync(drinking);

            await _repositoryManager.SaveAsync();

            return pet.Id;
        }

        public async Task FeedThePetAsync(Guid id)
        {
            //добавить проверку на свой-чужой
            var pet = await _repositoryManager.PetRepository.GetPetByIdAsync(id, true);
            if (pet == null)
                throw new Exception("pet not found");

            if(!_petConditionService.IsPetAlive(pet))
                throw new Exception("rest and peace");

            var feeding = new FeedingRecord() { PetId = id, FeedingDate = DateTime.Now };
            await _repositoryManager.FeedingHistoryRepository.CreateRecordAsync(feeding);

            pet.HungerLevel = HungerLevel.Full;
            await _repositoryManager.SaveAsync();
        }

        public async Task GiveADrinkToPetAsync(Guid id)
        {
            //добавить проверку на свой-чужой
            var pet = await _repositoryManager.PetRepository.GetPetByIdAsync(id, true);
            if (pet == null)
                throw new Exception("pet not found");

            if (!_petConditionService.IsPetAlive(pet))
                throw new Exception("rest and peace");

            var drinking = new DrinkingRecord() { PetId = id, DringkingDate = DateTime.Now };
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
            

        public async Task<PetDetailsDto> GetPetByIdAsync(Guid id)
        {
            var pet = await _repositoryManager.PetRepository.GetPetByIdAsync(id, true);
            if (pet == null)
                throw new Exception("pet not found");

            //UPDATE VITAL SIGNS AND SAVE
            await _petConditionService.UpdatePetFeedingAndDrinkingLevels(pet);

            var petForReturn = _mapper.Map<PetDetailsDto>(pet);

            return petForReturn;
        }

        public async Task UpdatePetNameAsync(Guid id, PetForUpdateDto petForUpdate)
        {
            var valResult = await _updatePetValidator.ValidateAsync(petForUpdate);
            if (!valResult.IsValid)
                throw new Exception("invalid data");

            var pet = await _repositoryManager.PetRepository.GetPetByIdAsync(id, true);
            if (pet == null)
                throw new Exception("pet not found");

            _mapper.Map(pet, petForUpdate);
            await _repositoryManager.SaveAsync();
        }
    }
}
