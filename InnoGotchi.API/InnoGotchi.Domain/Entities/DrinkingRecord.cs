namespace InnoGotchi.Domain.Entities
{
    public class DrinkingRecord
    {
        public Guid Id { get; set; }
        public DateTime DringkingDate { get; set; }
        public Guid PetId { get; set; }

        public Pet Pet { get; set; }
    }
}
