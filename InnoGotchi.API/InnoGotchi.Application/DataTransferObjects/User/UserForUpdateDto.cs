namespace InnoGotchi.Application.DataTransferObjects.User
{
    public class UserForUpdateDto
    {
        public string NewAvatar { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
