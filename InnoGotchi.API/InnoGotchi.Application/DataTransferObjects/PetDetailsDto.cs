using InnoGotchi.Domain.Enums;

namespace InnoGotchi.Application.DataTransferObjects
{
    public class PetDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public HungerLevel HungerLevel { get; set; }
        public ThirstyLevel ThirstyLevel { get; set; }
        public DateTime LastFed { get; set; }
        public DateTime LastDrank { get; set; }
        public int Age { get; set; }
        public bool IsAlvie { get; set; }
        public Body Body { get; set; }
        public Eye Eye { get; set; }
        public Nose Nose { get; set; }
        public Mouth Mouth { get; set; }
    }
}
