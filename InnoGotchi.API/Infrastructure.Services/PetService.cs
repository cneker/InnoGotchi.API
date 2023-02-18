using AutoMapper;
using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Pet;
using InnoGotchi.Application.Exceptions;
using InnoGotchi.Application.RequestFeatures;
using InnoGotchi.Domain.Entities;
using InnoGotchi.Domain.Enums;

namespace Infrastructure.Services
{
    public class PetService : IPetService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly IPetConditionService _petConditionService;
        private readonly ICheckUserService _checkUserService;

        public PetService(IRepositoryManager repositoryManager, IMapper mapper,
            IPetConditionService petConditionService, ICheckUserService checkUserService)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _petConditionService = petConditionService;
            _checkUserService = checkUserService;
        }

        public async Task<PetOverviewDto> CreatePetAsync(Guid userId, Guid farmId, PetForCreationDto petForCreation)
        {
            var farm = await CheckFarmExists(farmId);

            if(!_checkUserService.CheckWhetherUserIsOwner(farm, userId))
                throw new AccessDeniedException("You are not the owner of this farm");

            var pet = await _repositoryManager.PetRepository.GetPetByNameAsync(petForCreation.Name, false);
            if(pet != null)
                throw new IncorrectRequestException("This name has alredy registered");

            pet = _mapper.Map<Pet>(petForCreation);
            pet.FarmId = farmId;
            await _repositoryManager.PetRepository.CreatePetAsync(pet);

            var feeding = new HungryStateChanges() 
                { PetId = pet.Id, ChangesDate = DateTime.Now, HungerState = HungerLevel.Normal };
            var drinking = new ThirstyStateChanges() 
                { PetId = pet.Id, ChangesDate = DateTime.Now, ThirstyState = ThirstyLevel.Normal };
            await _repositoryManager.FeedingHistoryRepository.CreateRecordAsync(feeding);
            await _repositoryManager.DrinkingHistoryRepository.CreateRecordAsync(drinking);

            await _repositoryManager.SaveAsync();

            return _mapper.Map<PetOverviewDto>(pet);
        }

        public async Task FeedThePetAsync(Guid userId, Guid farmId, Guid id)
        {
            var farm = await CheckFarmExists(farmId);

            if(!_checkUserService.CheckWhetherUserIsCollaborator(farm, userId))
                if(!_checkUserService.CheckWhetherUserIsOwner(farm, userId))
                    throw new AccessDeniedException("You are not the owner or collaborator of this farm");

            var pet = await _repositoryManager.PetRepository.GetPetByIdAsync(id, true);
            if (pet == null)
                throw new NotFoundException("Pet not found");

            if(!_petConditionService.IsPetAlive(pet))
                throw new PetIsDeadException("Rest and peace");

            var feeding = new HungryStateChanges() 
                { PetId = id, ChangesDate = DateTime.Now, HungerState = HungerLevel.Full, IsFeeding = true };
            await _repositoryManager.FeedingHistoryRepository.CreateRecordAsync(feeding);

            pet.HungerLevel = HungerLevel.Full;
            await _repositoryManager.SaveAsync();
        }

        public async Task GiveADrinkToPetAsync(Guid userId, Guid farmId, Guid id)
        {
            var farm = await CheckFarmExists(farmId);

            if (!_checkUserService.CheckWhetherUserIsCollaborator(farm, userId))
                if (!_checkUserService.CheckWhetherUserIsOwner(farm, userId))
                    throw new AccessDeniedException("You are not the owner or collaborator of this farm");

            var pet = await _repositoryManager.PetRepository.GetPetByIdAsync(id, true);
            if (pet == null)
                throw new NotFoundException("Pet not found");

            if (!_petConditionService.IsPetAlive(pet))
                throw new PetIsDeadException("Rest and peace");

            var drinking = new ThirstyStateChanges() 
                { PetId = id, ChangesDate = DateTime.Now, ThirstyState = ThirstyLevel.Full, IsDrinking = true };
            await _repositoryManager.DrinkingHistoryRepository.CreateRecordAsync(drinking);

            pet.ThirstyLevel = ThirstyLevel.Full;
            await _repositoryManager.SaveAsync();
        }

        public async Task<PetPagingDto> GetAllPetsAsync(PetParameters petParameters)
        {
            var pets = await _repositoryManager.PetRepository.GetAllPetsAsync(petParameters, true);
            //UPDATE VITAL SIGNS AND SAVE
            foreach (var pet in pets)
                await _petConditionService.UpdatePetFeedingAndDrinkingLevels(pet);

            var petPagingDto = new PetPagingDto
            {
                MetaData = pets.MetaData,
                PetOverviewDtos = _mapper.Map<IEnumerable<PetDetailsDto>>(pets)
            };

            return petPagingDto;
        }

        public async Task<PetDetailsDto> GetPetByIdAsync(Guid userId, Guid farmId, Guid id)
        {
            var farm = await CheckFarmExists(farmId);

            if (!_checkUserService.CheckWhetherUserIsCollaborator(farm, userId))
                if (!_checkUserService.CheckWhetherUserIsOwner(farm, userId))
                    throw new AccessDeniedException("You are not the owner or collaborator of this farm");

            var pet = await _repositoryManager.PetRepository.GetPetByIdAsync(id, true);
            if (pet == null)
                throw new NotFoundException("Pet not found");
            //UPDATE VITAL SIGNS AND SAVE
            await _petConditionService.UpdatePetFeedingAndDrinkingLevels(pet);

            var petForReturn = _mapper.Map<PetDetailsDto>(pet);

            return petForReturn;
        }

        public async Task UpdatePetNameAsync(Guid userId, Guid farmId, Guid id, PetForUpdateDto petForUpdate)
        {
            var farm = await CheckFarmExists(farmId);

            if(!_checkUserService.CheckWhetherUserIsOwner(farm, userId))
                throw new AccessDeniedException("You are not the owner of this farm");

            var pet = await _repositoryManager.PetRepository.GetPetByIdAsync(id, true);
            if (pet == null)
                throw new NotFoundException("Pet not found");

            if (pet.Name == petForUpdate.Name)
                throw new IncorrectRequestException("This name has alredy registered");

            _mapper.Map(petForUpdate, pet);
            await _repositoryManager.SaveAsync();
        }

        private async Task<Farm> CheckFarmExists(Guid farmId)
        {
            var farm = await _repositoryManager.FarmRepository.GetFarmByIdAsync(farmId, false);

            if (farm == null)
                throw new NotFoundException("Farm not found");

            return farm;
        }
    }
}
