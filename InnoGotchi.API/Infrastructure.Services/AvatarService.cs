using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.User;

namespace Infrastructure.Services
{
    public class AvatarService : IAvatarService
    {
        private const string defaultAvatar = "default.jpg";
        public async Task<string> CreateImageAsync(Guid userId, AvatarChangingDto avatarDto)
        {
            var imageName = $"{userId}-{avatarDto.FileName}";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "avatars", imageName);
            var bytes = Convert.FromBase64String(avatarDto.Base64Image);

            using var stream = File.Create(path);
            stream.Write(bytes);

            return imageName;
        }

        public void DeleteOldImage(string avatarPath)
        {
            if (avatarPath != defaultAvatar)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "avatars", avatarPath);
                File.Delete(path);
            }
        }
    }
}
