namespace InnoGotchi.Domain.Entities
{
    public class Farm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid UserId { get; set; }

        public User User { get; set; }
        public ICollection<Pet> Pets { get; set; }
        public ICollection<User> Collaborators { get; set; }
    }
}
