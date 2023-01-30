namespace InnoGotchi.Domain.Entities
{
    public class HungryStateChanges
    {
        public Guid Id { get; set; }
        public DateTime ChangesDate { get; set; }
        public bool IsFeeding { get; set; }
        public Guid PetId { get; set; }

        public Pet Pet { get; set; }
    }
}
