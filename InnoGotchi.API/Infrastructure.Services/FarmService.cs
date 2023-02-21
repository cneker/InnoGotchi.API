using AutoMapper;
using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Farm;
using InnoGotchi.Application.DataTransferObjects.User;
using InnoGotchi.Application.Exceptions;
using InnoGotchi.Domain.Entities;

namespace Infrastructure.Services
{
    public class FarmService : IFarmService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly IGenerateFarmStatisticsService _generatetStatistics;
        private readonly IPetConditionService _petConditionService;
        private readonly ICheckUserService _checkUserService;

        public FarmService(IRepositoryManager repositoryManager, IMapper mapper,
            IGenerateFarmStatisticsService generateStatistics, IPetConditionService petConditionService,
            ICheckUserService checkUserService)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _generatetStatistics = generateStatistics;
            _petConditionService = petConditionService;
            _checkUserService = checkUserService;
        }

        public async Task<IEnumerable<FarmOverviewDto>> GetFarmsOverviewAsync() =>
            _mapper.Map<IEnumerable<FarmOverviewDto>>(await _repositoryManager.FarmRepository.GetFarmsAsync(false));

        public async Task<FarmOverviewDto> CreateFarmAsync(Guid userId, FarmForCreationDto farmForCreation)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(userId, false);
            if (user == null)
                throw new NotFoundException("User not found");

            if (user.UserFarm != null)
                throw new AlreadyExistsException("This user already has a farm");

            var farm = await _repositoryManager.FarmRepository.GetFarmByNameAsync(farmForCreation.Name, false);
            if (farm != null)
                throw new IncorrectRequestException("This name has alredy registered");

            farm = _mapper.Map<Farm>(farmForCreation);
            farm.UserId = userId;
            await _repositoryManager.FarmRepository.CreateFarm(farm);
            await _repositoryManager.SaveAsync();

            return _mapper.Map<FarmOverviewDto>(farm);
        }

        public async Task<FarmDetailsDto> GetFarmDetailsByFarmIdAsync(Guid userId, Guid farmId)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(userId, false);
            if (user == null)
                throw new NotFoundException("User not found");

            var farm = await _repositoryManager.FarmRepository.GetFarmByIdAsync(farmId, true);
            if (farm == null)
                throw new NotFoundException("Farm not found");

            if (!_checkUserService.CheckWhetherUserIsCollaborator(farm, userId))
                if (!_checkUserService.CheckWhetherUserIsOwner(farm, userId))
                    throw new AccessDeniedException("You are not the owner or collaborator of this farm");

            await _petConditionService.UpdatePetsFeedingAndDrinkingLevelsByFarm(farm);

            var farmForReturn = _mapper.Map<FarmDetailsDto>(farm);

            return farmForReturn;
        }

        public async Task<FarmOverviewDto> GetFarmOverviewByIdAsync(Guid userId)
        {
            var farm = await _repositoryManager.FarmRepository.GetFarmByUserIdAsync(userId, false);
            if (farm == null)
                throw new NotFoundException("Farm not found");

            var farmForReturn = _mapper.Map<FarmOverviewDto>(farm);

            return farmForReturn;
        }

        public async Task<FarmStatisticsDto> GetFarmStatisticsByFarmIdAsync(Guid userId, Guid farmId)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(userId, false);
            if (user == null)
                throw new NotFoundException("User not found");

            var farm = await _repositoryManager.FarmRepository.GetFarmByIdAsync(farmId, true);
            if (farm == null)
                throw new NotFoundException("Farm not found");

            if (!_checkUserService.CheckWhetherUserIsCollaborator(farm, userId))
                if (!_checkUserService.CheckWhetherUserIsOwner(farm, userId))
                    throw new AccessDeniedException("You are not the owner or collaborator of this farm");

            await _petConditionService.UpdatePetsFeedingAndDrinkingLevelsByFarm(farm);

            var farmForReturn = _mapper.Map<FarmStatisticsDto>(farm);

            return await _generatetStatistics.GenerateStatistics(farmForReturn);
        }

        public async Task<IEnumerable<FarmOverviewDto>> GetFriendsFarmsAsync(Guid userId)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(userId, false);
            if (user == null)
                throw new NotFoundException("User not found");

            var farms = _mapper.Map<IEnumerable<FarmOverviewDto>>(user.FriendsFarms);

            return farms;
        }

        public async Task InviteFriendAsync(Guid userId, Guid farmId, UserForInvitingDto userForInviting)
        {
            var friend = await _repositoryManager.UserRepository.
                GetUserByEmailAsync(userForInviting.Email, false);
            if (friend == null)
                throw new NotFoundException("User not found");

            var farm = await _repositoryManager.FarmRepository.GetFarmByIdAsync(farmId, true);
            if (farm == null)
                throw new NotFoundException("Farm not found");

            if (farm.Collaborators.SingleOrDefault(u => u.Id == friend.Id) != null)
                throw new AlreadyExistsException("This user is already your friend");

            if (friend.Id == userId)
                throw new IncorrectRequestException("You cannot be a collaborator on your own farm");

            farm.Collaborators.Add(friend);
            await _repositoryManager.SaveAsync();
        }

        public async Task UpdateFarmNameAsync(Guid userId, Guid farmId, FarmForUpdateDto farmForUpdate)
        {
            var farm = await _repositoryManager.FarmRepository.GetFarmByIdAsync(farmId, true);
            if (farm == null)
                throw new NotFoundException("Farm not found");

            if (farm.Name == farmForUpdate.Name)
                throw new IncorrectRequestException("This name has alredy registered");

            _mapper.Map(farmForUpdate, farm);
            await _repositoryManager.SaveAsync();
        }

        public async Task DeleteFarmById(Guid id)
        {
            var farm = await _repositoryManager.FarmRepository.GetFarmByUserIdAsync(id, true);
            if (farm == null)
                throw new NotFoundException("Farm not found");

            _repositoryManager.FarmRepository.DeleteFarm(farm);
            await _repositoryManager.SaveAsync();
        }
    }
}
