using Infrastructure.Persistance;
using InnoGotchi.Application.Contracts.Services;
using InnoGotchi.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InnoGotchi.IntegrationTests
{
    public class WebApplicationFixture : IDisposable
    {
        private readonly string _connectionString
            = "server=(localdb)\\mssqllocaldb; database=InnoGotchiIntegrationTests; Integrated Security = true";
        private readonly WebApplicationFactory<Program> _webApp;
        public AppDbContext Context { get; private set; }
        public HttpClient Client { get; private set; }

        public WebApplicationFixture()
        {
            _webApp = new WebApplicationFactory<Program>().WithWebHostBuilder(conf =>
            {
                conf.ConfigureServices(services =>
                {
                    var dbContextDescriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    services.Remove(dbContextDescriptor);
                    services.AddDbContext<AppDbContext>(opts =>
                    {
                        opts.UseSqlServer(_connectionString);
                    });
                });
            });
            InitContext();
            Client = _webApp.CreateClient();
        }

        public string GenerateToken(User user)
        {
            var scope = _webApp.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IGenerateTokenService>();
            return service.GenerateToken(user);
        }

        public string GeneratePasswordHash(string password)
        {
            var scope = _webApp.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
            return service.CreatePasswordHash(password);
        }

        private void InitContext()
        {
            var scope = _webApp.Services.CreateScope();
            Context = scope.ServiceProvider.GetService<AppDbContext>();
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Client.Dispose();
            Context.Dispose();
            _webApp.Dispose();
        }
    }
}
