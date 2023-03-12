namespace InnoGotchi.Application.DataTransferObjects
{
    public class AccessTokenDto
    {
        public Guid UserId { get; set; }
        public string AccessToken { get; set; }
    }
}
