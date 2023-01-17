namespace InnoGotchi.Application.DataTransferObjects.User
{
    //later change to UserDetailsDto with Avatar field
    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
