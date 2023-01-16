using InnoGotchi.Domain.Enums;

namespace InnoGotchi.Domain.Entities
{
    public class Pet
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime DeathDay { get; set; }
        public int HappynessDayCount { get; set; }
        public HungerLevel HungerLevel { get; set; }
        public ThirstyLevel ThirstyLevel { get; set; }
        public DateTime LastFed { get; set; }
        public DateTime LastDrank { get; set; }
        public Guid PetBodyId { get; set; }
        public Guid FramId { get; set; }

        public PetBody PetBody { get; set; }
        public Farm Farm { get; set; }
    }
}
