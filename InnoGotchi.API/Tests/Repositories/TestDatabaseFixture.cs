using Infrastructure.Persistance;
using InnoGotchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Tests.Repositories
{
    public class TestDatabaseFixture : IDisposable
    {
        private readonly string _connectionString
            = "server=(localdb)\\mssqllocaldb; database=InnoGotchiTests; Integrated Security = true";
        private readonly Fixture _fixture;
        public AppDbContext Context { get; init; }

        private AppDbContext CreateContext() =>
            new AppDbContext(
                new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(_connectionString)
                    .Options);

        public TestDatabaseFixture()
        {
            _fixture = new Fixture();
            Context = CreateContext();
            Context.Database.EnsureCreated();
            Cleanup();

            Initialize();
        }

        private void Initialize()
        {
            var users = new List<User>
            {
                _fixture.Build<User>()
                    .With(u => u.Id, Guid.Parse("8cd6c906-81c5-4717-8428-bfebe34d0f09"))
                    .With(u => u.Email, "email1@mail.com")
                    .Without(u => u.FriendsFarms)
                    .Without(u => u.UserFarm)
                    .Create(),
                _fixture.Build<User>()
                    .Without(u => u.FriendsFarms)
                    .Without(u => u.UserFarm)
                    .Create(),
                _fixture.Build<User>()
                    .With(u => u.Email, "email2@mail.com")
                    .Without(u => u.FriendsFarms)
                    .Without(u => u.UserFarm)
                    .Create()
            };

            Context.Users.AddRange(users);
            Context.SaveChanges();

            var farms = new List<Farm>
            {
                _fixture.Build<Farm>()
                    .With(f => f.UserId, users[0].Id)
                    .With(f => f.Collaborators, new List<User>())
                    .Without(f => f.User)
                    .Without(f => f.Pets)
                    .Create(),
                _fixture.Build<Farm>()
                    .With(f => f.UserId, users[1].Id)
                    .With(f => f.Collaborators, new List<User>())
                    .Without(f => f.User)
                    .Without(f => f.Pets)
                    .Create(),
            };

            Context.Farms.AddRange(farms);
            Context.SaveChanges();

            farms[0].Collaborators.Add(users[1]);
            farms[0].Collaborators.Add(users[2]);

            farms[1].Collaborators.Add(users[0]);

            Context.SaveChanges();
        }

        private void Cleanup()
        {
            Context.Farms.RemoveRange(Context.Farms);
            Context.Users.RemoveRange(Context.Users);
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}
