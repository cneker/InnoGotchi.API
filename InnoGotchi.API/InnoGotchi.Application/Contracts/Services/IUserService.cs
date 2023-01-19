using InnoGotchi.Application.DataTransferObjects.User;

namespace InnoGotchi.Application.Contracts.Services
{
    public interface IUserService
    {
        Task<Guid> CreateUserAsync(UserForRegistrationDto userForReg);
        Task<IEnumerable<UserInfoDto>> GetUsersInfoAsync();
        Task<UserInfoDto> GetUserInfoByIdAsync(Guid id);
        void DeleteUserById(Guid id);
        Task UpdateUserInfoAsync(Guid id, UserForUpdateDto userForUpdate);
        Task UpdatePasswordAsync(Guid id, UserForUpdateDto userForUpdate);
    }
}
