using InnoGotchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configurations
{
    public class FarmConfiguration : IEntityTypeConfiguration<Farm>
    {
        public void Configure(EntityTypeBuilder<Farm> builder)
        {
            builder.HasMany(f => f.Pets)
                .WithOne(p => p.Farm)
                .HasForeignKey(p => p.FarmId);
        }
    }
}
