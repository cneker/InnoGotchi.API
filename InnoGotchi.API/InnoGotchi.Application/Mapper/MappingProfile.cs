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
            CreateMap<UserForRegistrationDto, User>()
                .ForMember(u => u.Id, opt => opt.Ignore())
                .ForMember(u => u.AvatarPath, opt => opt.Ignore())
                .ForMember(u => u.PasswordHash, opt => opt.Ignore())
                .ForMember(u => u.UserFarm, opt => opt.Ignore())
                .ForMember(u => u.FriendsFarms, opt => opt.Ignore())
                .ForMember(u => u.Role, opt => opt.Ignore());
            CreateMap<User, UserInfoDto>();
            CreateMap<User, UserInfoForLayoutDto>();
            CreateMap<UserInfoForUpdateDto, User>()
                .ForMember(u => u.Id, opt => opt.Ignore())
                .ForMember(u => u.Email, opt => opt.Ignore())
                .ForMember(u => u.PasswordHash, opt => opt.Ignore())
                .ForMember(u => u.UserFarm, opt => opt.Ignore())
                .ForMember(u => u.FriendsFarms, opt => opt.Ignore())
                .ForMember(u => u.Role, opt => opt.Ignore())
                .ForMember(u => u.AvatarPath, opt => opt.Ignore());

            CreateMap<Farm, FarmOverviewDto>()
                .ForMember(f => f.PetsCount, opt => opt.MapFrom(f => f.Pets.Count));
            CreateMap<Farm, FarmDetailsDto>()
                .ForMember(f => f.PetsCount, opt => opt.MapFrom(f => f.Pets.Count));
            CreateMap<Farm, FarmStatisticsDto>()
                .ForMember(f => f.AlivePetsCount, opt => opt.Ignore())
                .ForMember(f => f.DeadPetsCount, opt => opt.Ignore())
                .ForMember(f => f.AverageFeedingPeriod, opt => opt.Ignore())
                .ForMember(f => f.AverageThirstQuenchingPeriod, opt => opt.Ignore())
                .ForMember(f => f.AveragePetsHappinessDaysCount, opt => opt.Ignore())
                .ForMember(f => f.AveragePetsAge, opt => opt.Ignore());
            CreateMap<FarmForUpdateDto, Farm>()
                .ForMember(f => f.Id, opt => opt.Ignore())
                .ForMember(f => f.UserId, opt => opt.Ignore())
                .ForMember(f => f.User, opt => opt.Ignore())
                .ForMember(f => f.Pets, opt => opt.Ignore())
                .ForMember(f => f.Collaborators, opt => opt.Ignore());
            CreateMap<FarmForCreationDto, Farm>()
                .ForMember(f => f.Id, opt => opt.Ignore())
                .ForMember(f => f.UserId, opt => opt.Ignore())
                .ForMember(f => f.User, opt => opt.Ignore())
                .ForMember(f => f.Pets, opt => opt.Ignore())
                .ForMember(f => f.Collaborators, opt => opt.Ignore());

            CreateMap<Pet, PetDetailsDto>()
                .ForMember(p => p.Age,
                    opt => opt.MapFrom(p => petConditionService.CalculateAge(p)))
                .ForMember(p => p.IsAlive,
                    opt => opt.MapFrom(p => petConditionService.IsPetAlive(p)))
                .ForMember(p => p.HungerLevel,
                    opt => opt.MapFrom(p => p.HungerLevel.ToString()))
                .ForMember(p => p.ThirstyLevel,
                    opt => opt.MapFrom(p => p.ThirstyLevel.ToString()));
            CreateMap<Pet, PetOverviewDto>()
                .ForMember(p => p.Age,
                    opt => opt.MapFrom(p => petConditionService.CalculateAge(p)))
                .ForMember(p => p.IsAlive,
                    opt => opt.MapFrom(p => petConditionService.IsPetAlive(p)))
                .ForMember(p => p.HungerLevel,
                    opt => opt.MapFrom(p => p.HungerLevel.ToString()))
                .ForMember(p => p.ThirstyLevel,
                    opt => opt.MapFrom(p => p.ThirstyLevel.ToString()));
            CreateMap<PetForCreationDto, Pet>()
                .ForMember(p => p.Id, opt => opt.Ignore())
                .ForMember(p => p.Birthday, opt => opt.Ignore())
                .ForMember(p => p.DeathDay, opt => opt.Ignore())
                .ForMember(p => p.HappynessDayCount, opt => opt.Ignore())
                .ForMember(p => p.HungerLevel, opt => opt.Ignore())
                .ForMember(p => p.ThirstyLevel, opt => opt.Ignore())
                .ForMember(p => p.FarmId, opt => opt.Ignore())
                .ForMember(p => p.Farm, opt => opt.Ignore())
                .ForMember(p => p.HungryStateChangesHistory, opt => opt.Ignore())
                .ForMember(p => p.ThirstyStateChangesHistory, opt => opt.Ignore());
            CreateMap<PetForUpdateDto, Pet>()
                .ForMember(p => p.Id, opt => opt.Ignore())
                .ForMember(p => p.Birthday, opt => opt.Ignore())
                .ForMember(p => p.DeathDay, opt => opt.Ignore())
                .ForMember(p => p.HappynessDayCount, opt => opt.Ignore())
                .ForMember(p => p.HungerLevel, opt => opt.Ignore())
                .ForMember(p => p.ThirstyLevel, opt => opt.Ignore())
                .ForMember(p => p.Body, opt => opt.Ignore())
                .ForMember(p => p.Eye, opt => opt.Ignore())
                .ForMember(p => p.Nose, opt => opt.Ignore())
                .ForMember(p => p.Mouth, opt => opt.Ignore())
                .ForMember(p => p.FarmId, opt => opt.Ignore())
                .ForMember(p => p.Farm, opt => opt.Ignore())
                .ForMember(p => p.HungryStateChangesHistory, opt => opt.Ignore())
                .ForMember(p => p.ThirstyStateChangesHistory, opt => opt.Ignore());
        }
    }
}
