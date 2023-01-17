using InnoGotchi.Application.DataTransferObjects.Pet;

namespace InnoGotchi.Application.DataTransferObjects.Farm
{
    public class FarmDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<PetOverviewDto> Pets { get; set; }
    }
}
