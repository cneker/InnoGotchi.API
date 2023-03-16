using Infrastructure.Persistance.Configurations;
using InnoGotchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance
{
    public class AppDbContext : DbContext
    {
        public DbSet<Farm> Farms { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<HungryStateChanges> HungryStateChangesHistory { get; set; }
        public DbSet<ThirstyStateChanges> ThirstyStateChangesHistory { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new FarmConfiguration());
            builder.ApplyConfiguration(new PetConfiguration());
            builder.ApplyConfiguration(new HungryStateChangesConfiguration());
            builder.ApplyConfiguration(new ThirstyStateChangesConfiguration());
        }
    }
}
