namespace InnoGotchi.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AvatarPath { get; set; }  
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }

        public Farm UserFarm { get; set; }
        public ICollection<Farm> FriendsFarms { get; set; }
    }
}
