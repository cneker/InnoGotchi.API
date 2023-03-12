using InnoGotchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configurations
{
    public class HungryStateChangesConfiguration : IEntityTypeConfiguration<HungryStateChanges>
    {
        public void Configure(EntityTypeBuilder<HungryStateChanges> builder)
        {
            builder.Property(p => p.IsFeeding)
                .HasDefaultValue(false);
        }
    }
}
