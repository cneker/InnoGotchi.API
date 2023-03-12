namespace InnoGotchi.Application.DataTransferObjects.Pet
{
    public class PetOverviewDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string HungerLevel { get; set; }
        public string ThirstyLevel { get; set; }
        public int Age { get; set; }
        public double HappinessDayCount { get; set; }
        public bool IsAlive { get; set; }
    }
}
