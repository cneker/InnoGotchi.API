namespace InnoGotchi.Application.Contracts.Services
{
    public interface ICalculatePetAgeService
    {
        int CalculateAge(DateTime birthday, DateTime? deathday);
    }
}
