using InnoGotchi.Domain.Enums;

namespace InnoGotchi.Application.DataTransferObjects.Pet
{
    public class PetForCreationDto
    {
        public string Name { get; set; }
        public Body Body { get; set; }
        public Eye Eye { get; set; }
        public Nose Nose { get; set; }
        public Mouth Mouth { get; set; }
    }
}
