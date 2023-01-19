using AutoMapper;
using InnoGotchi.Application.DataTransferObjects.Farm;
using InnoGotchi.Application.DataTransferObjects.User;
using InnoGotchi.Domain.Entities;

namespace InnoGotchi.Application.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserForRegistrationDto, User>();
            CreateMap<UserInfoDto, User>();

            CreateMap<Farm, FarmOverviewDto>();
            CreateMap<Farm, FarmDetailsDto>();
        }
    }
}
