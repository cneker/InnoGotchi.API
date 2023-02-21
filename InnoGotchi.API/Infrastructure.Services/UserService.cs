using AutoMapper;
using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.User;
using InnoGotchi.Application.Exceptions;
using InnoGotchi.Domain.Entities;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IAuthenticationService _authService;
        private readonly IMapper _mapper;
        private readonly IAvatarService _avatarService;


        public UserService(IRepositoryManager repositoryManager,
            IAuthenticationService authenticationService, IMapper mapper, IAvatarService avatarService)
        {
            _repositoryManager = repositoryManager;
            _authService = authenticationService;
            _mapper = mapper;
            _avatarService = avatarService;
        }

        public async Task<UserInfoDto> CreateUserAsync(UserForRegistrationDto userForReg)
        {
            var user = await _repositoryManager.UserRepository.GetUserByEmailAsync(userForReg.Email, false);
            if (user != null)
                throw new AlreadyExistsException("The email has already registered");

            user = _mapper.Map<User>(userForReg);
            user.PasswordHash = _authService.CreatePasswordHash(userForReg.Password);
            await _repositoryManager.UserRepository.CreateUser(user);
            await _repositoryManager.SaveAsync();

            return _mapper.Map<UserInfoDto>(user);
        }

        public async Task<IEnumerable<UserInfoDto>> GetUsersInfoAsync() =>
            _mapper.Map<IEnumerable<UserInfoDto>>(await _repositoryManager.UserRepository.GetUsersAsync(false));

        public async Task<UserInfoForLayoutDto> GetUserInfoForLayoutByIdAsync(Guid id)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(id, false);
            if (user == null)
                throw new NotFoundException("User not found");

            var userInfo = _mapper.Map<UserInfoForLayoutDto>(user);

            return userInfo;
        }

        public async Task DeleteUserById(Guid id)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(id, false);
            if (user == null)
                throw new NotFoundException("User not found");

            if (user.UserFarm != null)
                _repositoryManager.FarmRepository.DeleteFarm(user.UserFarm);
            _repositoryManager.UserRepository.DeleteUser(user);

            await _repositoryManager.SaveAsync();
        }

        public async Task<UserInfoDto> GetUserInfoByIdAsync(Guid id)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(id, false);
            if (user == null)
                throw new NotFoundException("User not found");

            var userInfo = _mapper.Map<UserInfoDto>(user);

            return userInfo;
        }

        public async Task UpdatePasswordAsync(Guid id, PasswordChangingDto passwordForUpdate)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(id, true);
            if (user == null)
                throw new NotFoundException("User not found");

            user.PasswordHash = _authService.CreatePasswordHash(passwordForUpdate.NewPassword);

            await _repositoryManager.SaveAsync();
        }

        public async Task UpdateUserInfoAsync(Guid id, UserInfoForUpdateDto userForUpdate)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(id, true);
            if (user == null)
                throw new NotFoundException("User not found");

            _mapper.Map(userForUpdate, user);

            await _repositoryManager.SaveAsync();
        }

        public async Task UpdateAvatarAsync(Guid id, AvatarChangingDto avatarDto)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(id, true);
            if (user == null)
                throw new NotFoundException("User not found");

            var imagePath = await _avatarService.CreateImageAsync(id, avatarDto);
            _avatarService.DeleteOldImage(user.AvatarPath);
            user.AvatarPath = imagePath;

            await _repositoryManager.SaveAsync();
        }
    }
}
