using AutoMapper;
using FluentValidation;
using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Farm;
using InnoGotchi.Application.DataTransferObjects.User;
using InnoGotchi.Domain.Entities;

namespace Infrastructure.Services
{
    public class FarmService : IFarmService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IValidator<FarmForCreationDto> _createFarmValidator;
        private readonly IValidator<FarmForUpdateDto> _updateFarmValidator;
        private readonly IValidator<UserForInvitingDto> _inviteUserValidator;
        private readonly IMapper _mapper;
        private readonly IGenerateFarmStatisticsService _generatetStatistics;
        private readonly IPetConditionService _petConditionService;

        public FarmService(IRepositoryManager repositoryManager, IValidator<FarmForCreationDto> createFarmValidator, 
            IValidator<FarmForUpdateDto> updateFarmValidator, IMapper mapper,
            IValidator<UserForInvitingDto> inviteUserValidator,
            IGenerateFarmStatisticsService generateStatistics, IPetConditionService petConditionService)
        {
            _repositoryManager = repositoryManager;
            _createFarmValidator = createFarmValidator;
            _updateFarmValidator = updateFarmValidator;
            _mapper = mapper;
            _inviteUserValidator = inviteUserValidator;
            _generatetStatistics = generateStatistics;
            _petConditionService = petConditionService;
        }

        public async Task<Guid> CreateFarmAsync(Guid userId, FarmForCreationDto farmForCreation)
        {
            var valResult = await _createFarmValidator.ValidateAsync(farmForCreation);
            if(!valResult.IsValid)
                throw new Exception("invalid data");

            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(userId, false);
            if(user == null)
                throw new Exception("user not found");

            var farm = _mapper.Map<Farm>(farmForCreation);
            farm.UserId = userId;
            await _repositoryManager.FarmRepository.CreateFarm(farm);
            await _repositoryManager.SaveAsync();

            return farm.Id;
        }

        public async Task<FarmDetailsDto> GetFarmDetailsByIdAsync(Guid userId)
        {
            var farm = await _repositoryManager.FarmRepository.GetFarmByUserIdAsync(userId, false);
            if (farm == null)
                throw new Exception("farm not found");

            //UPDATE VITAL SIGNS AND SAVE
            await _petConditionService.UpdatePetsFeedingAndDrinkingLevelsByFarm(farm.Id);

            var farmForReturn = _mapper.Map<FarmDetailsDto>(farm);
            farmForReturn.PetsCount = farm.Pets.Count;

            return farmForReturn;
        }

        public async Task<FarmOverviewDto> GetFarmOverviewByIdAsync(Guid userId)
        {
            var farm = await _repositoryManager.FarmRepository.GetFarmByUserIdAsync(userId, false);
            if (farm == null)
                throw new Exception("farm not found");

            var farmForReturn = _mapper.Map<FarmOverviewDto>(farm);

            return farmForReturn;
        }

        public async Task<FarmStatisticsDto> GetFarmStatisticsByIdAsync(Guid userId)
        {
            var farm = await _repositoryManager.FarmRepository.GetFarmByUserIdAsync(userId, false);
            if (farm == null)
                throw new Exception("farm not found");
            var farmForReturn = _mapper.Map<FarmStatisticsDto>(farm);

            return await _generatetStatistics.GenerateStatistics(farmForReturn);
        }

        public async Task<IEnumerable<FarmOverviewDto>> GetFriendsFarmsAsync(Guid userId)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(userId, false);
            if (user == null)
                throw new Exception("user not found");

            var farms = _mapper.Map<IEnumerable<FarmOverviewDto>>(user.FriendsFarms);

            return farms;
        }

        public async Task InviteFriendAsync(Guid userId, UserForInvitingDto userForInviting)
        {
            var valResult = await _inviteUserValidator.ValidateAsync(userForInviting);
            if (!valResult.IsValid)
                throw new Exception("invalid data");
            
            var friend = await _repositoryManager.UserRepository.
                GetUserByEmailAsync(userForInviting.Email, false);
            if(friend == null)
                throw new Exception("user not found");

            var farm = await _repositoryManager.FarmRepository.GetFarmByUserIdAsync(userId, true);
            if (farm == null)
                throw new Exception("farm not found");

            farm.Collaborators.Add(friend);
            await _repositoryManager.SaveAsync();
        }

        public async Task UpdateFarmNameAsync(Guid userId, FarmForUpdateDto farmForUpdate)
        {
            var valResult = await _updateFarmValidator.ValidateAsync(farmForUpdate);
            if (!valResult.IsValid)
                throw new Exception("invalid data");

            var farm = await _repositoryManager.FarmRepository.GetFarmByUserIdAsync(userId, true);
            if (farm == null)
                throw new Exception("farm not found");

            _mapper.Map(farmForUpdate, farm);
            await _repositoryManager.SaveAsync();
        }

        public async Task DeleteFarmById(Guid id)
        {
            var farm = await _repositoryManager.FarmRepository.GetFarmByIdAsync(id, true);
            if (farm == null)
                throw new Exception("farm not found");

            _repositoryManager.FarmRepository.DeleteFarm(farm);
        }
    }
}
