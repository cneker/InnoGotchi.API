namespace InnoGotchi.Application.DataTransferObjects.User
{
    public class PasswordChangingDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmedPassword { get; set; }
    }
}
