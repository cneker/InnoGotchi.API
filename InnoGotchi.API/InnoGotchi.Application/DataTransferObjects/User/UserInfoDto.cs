namespace InnoGotchi.Application.DataTransferObjects.User
{
    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string AvatarPath { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
