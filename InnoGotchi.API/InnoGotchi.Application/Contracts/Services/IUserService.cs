using InnoGotchi.Application.DataTransferObjects.User;

namespace InnoGotchi.Application.Contracts.Services
{
    public interface IUserService
    {
        Task<UserInfoDto> CreateUserAsync(UserForRegistrationDto userForReg);
        Task<IEnumerable<UserInfoDto>> GetUsersInfoAsync();
        Task<UserInfoDto> GetUserInfoByIdAsync(Guid id);
        Task<UserInfoForLayoutDto> GetUserInfoForLayoutByIdAsync(Guid id);
        Task DeleteUserById(Guid id);
        Task UpdateUserInfoAsync(Guid id, UserInfoForUpdateDto userForUpdate);
        Task UpdatePasswordAsync(Guid id, PasswordChangingDto passwordForUpdate);
        Task UpdateAvatarAsync(Guid id, AvatarChangingDto avatarDto);
    }
}
