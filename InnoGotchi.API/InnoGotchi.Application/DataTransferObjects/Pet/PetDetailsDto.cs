using InnoGotchi.Domain.Enums;

namespace InnoGotchi.Application.DataTransferObjects.Pet
{
    public class PetDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public HungerLevel HungerLevel { get; set; }
        public ThirstyLevel ThirstyLevel { get; set; }
        public int Age { get; set; }
        public bool IsAlive { get; set; }
        public Body Body { get; set; }
        public Eye Eye { get; set; }
        public Nose Nose { get; set; }
        public Mouth Mouth { get; set; }
    }
}
