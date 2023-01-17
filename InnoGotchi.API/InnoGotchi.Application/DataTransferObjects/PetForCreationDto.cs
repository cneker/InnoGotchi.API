using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.DataTransferObjects
{
    public class PetForCreationDto
    {
        public string Name { get; set; }
        public PetBody PetBody { get; set; }
    }
}
