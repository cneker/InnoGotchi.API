using AutoMapper;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.Mapper;
using InnoGotchi.Domain.Entities;

namespace Tests.Mapping
{
    public class MappingTests
    {
        [Fact]
        public void TestingMappings()
        {
            //Arrange
            var serviceMock = new Mock<IPetConditionService>();
            serviceMock.Setup(a => a.IsPetAlive(It.IsAny<Pet>()))
                .Returns(It.IsAny<bool>());
            serviceMock.Setup(a => a.CalculateAge(It.IsAny<Pet>()))
                .Returns(It.IsAny<int>());
            serviceMock.Setup(a => a.UpdatePetsFeedingAndDrinkingLevelsByFarm(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile(serviceMock.Object));
            }).CreateMapper();

            //Act and Assert
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
