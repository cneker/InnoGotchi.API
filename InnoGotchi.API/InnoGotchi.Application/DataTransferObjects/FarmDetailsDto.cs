namespace InnoGotchi.Application.DataTransferObjects
{
    public class FarmDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public ICollection<PetOverviewDto> Pets { get; set; }
    }
}
