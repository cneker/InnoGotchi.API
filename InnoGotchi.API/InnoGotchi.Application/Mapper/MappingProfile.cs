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
        public MappingProfile(IPetConditionService petConditionService)
        {
            CreateMap<UserForRegistrationDto, User>();
            CreateMap<UserInfoDto, User>();

            CreateMap<Farm, FarmOverviewDto>();
            CreateMap<Farm, FarmDetailsDto>()
                .BeforeMap(async (src, dst) => 
                    await petConditionService.UpdatePetsFeedingAndDrinkingLevelsByFarm(dst.Id));
            CreateMap<Farm, FarmStatisticsDto>();
            CreateMap<FarmForUpdateDto, Farm>();
            CreateMap<FarmForCreationDto, Farm>();

            CreateMap<Pet, PetDetailsDto>()
                .ForMember(p => p.Age,
                    opt => opt.MapFrom(p => petConditionService.CalculateAge(p)))
                .ForMember(p => p.IsAlive,
                    opt => opt.MapFrom(p => petConditionService.IsPetAlive(p)));
            CreateMap<Pet, PetOverviewDto>()
                .ForMember(p => p.Age, 
                    opt => opt.MapFrom(p => petConditionService.CalculateAge(p)))
                .ForMember(p => p.IsAlive, 
                    opt => opt.MapFrom(p => petConditionService.IsPetAlive(p)));
            CreateMap<PetForCreationDto, Pet>();
            CreateMap<PetForUpdateDto, Pet>();
        }
    }
}
