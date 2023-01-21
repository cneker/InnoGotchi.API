using InnoGotchi.Application.Contracts.Services;

namespace Infrastructure.Services
{
    public class CalculatePetAgeService : ICalculatePetAgeService
    {
        public int CalculateAge(DateTime birthday, DateTime? deathday) =>
            deathday != null ?
                (deathday - birthday).Value.Days / 7 :
                (DateTime.Now - birthday).Days / 7;
    }
}
