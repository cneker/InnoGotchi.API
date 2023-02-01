using InnoGotchi.Domain.Enums;

namespace InnoGotchi.Domain.Entities
{
    public class ThirstyStateChanges
    {
        public Guid Id { get; set; }
        public DateTime ChangesDate { get; set; }
        public bool IsDrinking { get; set; }
        public ThirstyLevel ThirstyState { get; set; }
        public Guid PetId { get; set; }

        public Pet Pet { get; set; }
    }
}
