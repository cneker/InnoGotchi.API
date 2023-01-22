using InnoGotchi.Domain.Enums;

namespace InnoGotchi.Application.DataTransferObjects.Pet
{
    public class PetOverviewDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public HungerLevel HungerLevel { get; set; }
        public ThirstyLevel ThirstyLevel { get; set; }
        public int Age { get; set; }
        public bool IsAlive { get; set; }
    }
}
