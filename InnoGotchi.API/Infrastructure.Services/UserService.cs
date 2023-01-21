using AutoMapper;
using FluentValidation;
using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.User;
using InnoGotchi.Domain.Entities;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IAuthenticationService _authService;
        private readonly IMapper _mapper;
        private readonly IValidator<UserForRegistrationDto> _regValidator;
        private readonly IValidator<UserForUpdateDto> _updateValidator;


        public UserService(IRepositoryManager repositoryManager, IAuthenticationService authenticationService, 
            IMapper mapper, IValidator<UserForRegistrationDto> regValidator, IValidator<UserForUpdateDto> updateValidator)
        {
            _repositoryManager = repositoryManager;
            _authService = authenticationService;
            _mapper = mapper;
            _regValidator = regValidator;
            _updateValidator = updateValidator;
        }

        public async Task<Guid> CreateUserAsync(UserForRegistrationDto userForReg)
        {
            var valResult = await _regValidator.ValidateAsync(userForReg);
            if (!valResult.IsValid)
                throw new Exception("invalid data");

            var user = await _repositoryManager.UserRepository.GetUserByEmailAsync(userForReg.Email, false);
            if(user != null)
                throw new Exception("the email has already registered");

            user = _mapper.Map<User>(userForReg);
            user.PasswordHash = _authService.CreatePasswordHash(userForReg.Password);
            await _repositoryManager.UserRepository.CreateUser(user);
            await _repositoryManager.SaveAsync();
            return user.Id;
        }

        public async Task<IEnumerable<UserInfoDto>> GetUsersInfoAsync() =>
            _mapper.Map<IEnumerable<UserInfoDto>>(await _repositoryManager.UserRepository.GetUsersAsync(false));

        public async void DeleteUserById(Guid id)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(id, false);
            if (user == null)
                throw new Exception("user not found");

            _repositoryManager.FarmRepository.DeleteFarm(user.UserFarm);
            _repositoryManager.UserRepository.DeleteUser(user);
        }

        public async Task<UserInfoDto> GetUserInfoByIdAsync(Guid id)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(id, false);
            if (user == null)
                throw new Exception("user not found");

            var userInfo = _mapper.Map<UserInfoDto>(user);

            return userInfo;
        }

        public async Task UpdatePasswordAsync(Guid id, UserForUpdateDto userForUpdate)
        {
            var valResult = await _updateValidator.ValidateAsync(userForUpdate);
            if (!valResult.IsValid)
                throw new Exception("invalid data");

            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(id, true);
            if (user == null)
                throw new Exception("user not found");

            user = _mapper.Map<User>(userForUpdate);

            user.PasswordHash = _authService.CreatePasswordHash(userForUpdate.NewPassword);
        }

        public async Task UpdateUserInfoAsync(Guid id, UserForUpdateDto userForUpdate)
        {
            var valResult = await _updateValidator.ValidateAsync(userForUpdate);
            if (!valResult.IsValid)
                throw new Exception("invalid data");

            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(id, true);
            if (user == null)
                throw new Exception("user not found");

            _mapper.Map(userForUpdate, user);
        }
    }
}
