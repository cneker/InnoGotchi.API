namespace InnoGotchi.Application.DataTransferObjects
{
    public class FarmStatisticsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int AlivePetsCount { get; set; }
        public int DeadPetsCount { get; set; }
        public int AverageFeedingPeriod { get; set; }
        public int AverageThirstQuenchingPeriod { get; set; }
        public int AveragePetsHappinessDaysCount { get; set; }
        public int AveragePetsAge { get; set; }
    }
}
