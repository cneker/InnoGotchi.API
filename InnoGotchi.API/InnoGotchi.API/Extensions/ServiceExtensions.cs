using AutoMapper;
using Infrastructure.Persistance;
using Infrastructure.Persistance.Repositories;
using Infrastructure.Services;
using InnoGotchi.API.Filters.ActionFilters;
using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.Mapper;
using Microsoft.EntityFrameworkCore;

namespace InnoGotchi.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services,
            IConfiguration configuration) =>
                services.AddDbContext<AppDbContext>(opts =>
                    opts.UseSqlServer(configuration.GetConnectionString("sqlConnection"), b =>
                        b.MigrationsAssembly("Infrastructure.Persistance")));

        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
            services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureAutoMapper(this IServiceCollection services) =>
            services.AddScoped(provider => new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile(provider.GetService<IPetConditionService>()));
            }).CreateMapper());

        public static void ConfigureAuthenticationService(this IServiceCollection services) =>
            services.AddScoped<IAuthenticationService, AuthenticationService>();

        public static void ConfigureFarmService(this IServiceCollection services) =>
            services.AddScoped<IFarmService, FarmService>();

        public static void ConfigureGenerateFarmStatisticsService(this IServiceCollection services) =>
            services.AddScoped<IGenerateFarmStatisticsService, GenerateFarmStatisticsService>();

        public static void ConfigureGenerateTokenService(this IServiceCollection services) =>
            services.AddScoped<IGenerateTokenService, GenerateTokenService>();

        public static void ConfigurePetConditionService(this IServiceCollection services) =>
            services.AddScoped<IPetConditionService, PetConditionService>();

        public static void ConfigurePetService(this IServiceCollection service) =>
            service.AddScoped<IPetService, PetService>();

        public static void ConfigureUserService(this IServiceCollection services) =>
            services.AddScoped<IUserService, UserService>();

        public static void ConfigurActionFilters(this IServiceCollection services)
        {
            services.AddScoped<ValidationFilterAttribute>();
        }
    }
}
