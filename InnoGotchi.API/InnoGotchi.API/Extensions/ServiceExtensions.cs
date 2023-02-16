using AutoMapper;
using Infrastructure.Persistance;
using Infrastructure.Persistance.Repositories;
using Infrastructure.Services;
using InnoGotchi.API.Filters.ActionFilters;
using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Application.Mapper;
using InnoGotchi.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

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

        public static void ConfigureAvatarService(this IServiceCollection services) =>
            services.AddScoped<IAvatarService, AvatarService>();

        public static void ConfigureCheckUserService(this IServiceCollection services) =>
            services.AddScoped<ICheckUserService, CheckUserService>();

        public static void ConfigurActionFilters(this IServiceCollection services)
        {
            services.AddScoped<ValidationFilterAttribute>();
            services.AddScoped<ExtractUserIdFilterAttribute>();
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var key = Environment.GetEnvironmentVariable("SECRET");

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
                    ValidAudience = jwtSettings.GetSection("validAudience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });
        }

        public static void ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy(Roles.User.ToString(), pol => pol.RequireRole(Roles.User.ToString()));
                opt.AddPolicy(Roles.Admin.ToString(), pol => pol.RequireRole(Roles.Admin.ToString()));
            });
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("InnoGotchi", new OpenApiInfo 
                {
                    Title = "InnoGotchi API",
                    Description = "Test task for Innowise",
                    Contact = new OpenApiContact
                    {
                        Name = "Stas Kotashevich",
                        Email = "staskotashevich@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/stas-kotashevich-6504451b6")
                    }
                });

                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Place to add JWT with Bearer",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer",
                        },
                        new List<string>()
                    }
                });
            });
        }
    }
}
