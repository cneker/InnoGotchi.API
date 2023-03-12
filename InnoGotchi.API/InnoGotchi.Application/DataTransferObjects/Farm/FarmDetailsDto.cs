using InnoGotchi.Application.DataTransferObjects.Pet;

namespace InnoGotchi.Application.DataTransferObjects.Farm
{
    public class FarmDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PetsCount { get; set; }

        public IEnumerable<PetOverviewDto> Pets { get; set; }
    }
}
