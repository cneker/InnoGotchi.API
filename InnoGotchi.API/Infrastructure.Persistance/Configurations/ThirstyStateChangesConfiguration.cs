using InnoGotchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configurations
{
    public class ThirstyStateChangesConfiguration : IEntityTypeConfiguration<ThirstyStateChanges>
    {
        public void Configure(EntityTypeBuilder<ThirstyStateChanges> builder)
        {
            builder.Property(p => p.IsDrinking)
                .HasDefaultValue(false);
        }
    }
}
