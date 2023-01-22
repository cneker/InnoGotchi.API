namespace InnoGotchi.Domain.Entities
{
    public class FeedingRecord
    {
        public Guid Id { get; set; }
        public DateTime FeedingDate { get; set; }
        public Guid PetId { get; set; }

        public Pet Pet { get; set; }
    }
}
