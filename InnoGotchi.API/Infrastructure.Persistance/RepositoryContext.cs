using InnoGotchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance
{
    public class RepositoryContext : DbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Farm> Farms { get; set; }
        DbSet<Pet> Pets { get; set; }

        public RepositoryContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
            //configurations for each dbset
        }
    }
}
