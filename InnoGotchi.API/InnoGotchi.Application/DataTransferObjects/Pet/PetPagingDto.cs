using InnoGotchi.Application.RequestFeatures;

namespace InnoGotchi.Application.DataTransferObjects.Pet
{
    public class PetPagingDto
    {
        public IEnumerable<PetDetailsDto> PetOverviewDtos { get; set; }
        public MetaData MetaData { get; set; }
    }
}
