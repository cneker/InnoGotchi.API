using AutoMapper;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.DataTransferObjects.Farm;
using InnoGotchi.Application.DataTransferObjects.Pet;
using InnoGotchi.Application.DataTransferObjects.User;
using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile(ICalculatePetAgeService calculatePetAgeService)
        {
            CreateMap<UserForRegistrationDto, User>();
            CreateMap<UserInfoDto, User>();

            CreateMap<Farm, FarmOverviewDto>();
            CreateMap<Farm, FarmDetailsDto>();
            CreateMap<Farm, FarmStatisticsDto>();
            CreateMap<FarmForUpdateDto, Farm>();
            CreateMap<FarmForCreationDto, Farm>();

            CreateMap<Pet, PetDetailsDto>()
                 .ForMember(p => p.Age,
                    opt => opt.MapFrom(p => calculatePetAgeService.CalculateAge(p.Birthday, p.DeathDay)))
                .ForMember(p => p.IsAlive,
                    opt => opt.MapFrom(p => p.DeathDay == null));
            CreateMap<Pet, PetOverviewDto>()
                .ForMember(p => p.Age, 
                    opt => opt.MapFrom(p => calculatePetAgeService.CalculateAge(p.Birthday, p.DeathDay)))
                .ForMember(p => p.IsAlive, 
                    opt => opt.MapFrom(p => p.DeathDay == null));
            CreateMap<PetForCreationDto, Pet>();
            CreateMap<PetForUpdateDto, Pet>();
        }
    }
}
