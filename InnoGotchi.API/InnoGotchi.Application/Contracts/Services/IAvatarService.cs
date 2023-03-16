using InnoGotchi.Application.DataTransferObjects.User;

namespace InnoGotchi.Application.Contracts.Services
{
    public interface IAvatarService
    {
        Task<string> CreateImageAsync(Guid userId, AvatarChangingDto avatarDto);
        void DeleteOldImage(string avatarPath);
    }
}
