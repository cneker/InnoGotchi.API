using InnoGotchi.Domain.Entities;
using InnoGotchi.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configurations
{
    public class PetConfiguration : IEntityTypeConfiguration<Pet>
    {
        public void Configure(EntityTypeBuilder<Pet> builder)
        {
            builder.HasMany(p => p.HungryStateChangesHistory)
                .WithOne(fr => fr.Pet)
                .HasForeignKey(fr => fr.PetId);
            builder.HasMany(p => p.ThirstyStateChangesHistory)
                .WithOne(dr => dr.Pet)
                .HasForeignKey(dr => dr.PetId);
            builder.HasIndex(p => p.Name)
                .IsUnique();

            builder.Property(p => p.Birthday)
                .HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.HappynessDayCount)
                .HasDefaultValue(0);
            builder.Property(p => p.HungerLevel)
                .HasDefaultValue(HungerLevel.Normal);
            builder.Property(p => p.ThirstyLevel)
                .HasDefaultValue(ThirstyLevel.Normal);
        }
    }
}
