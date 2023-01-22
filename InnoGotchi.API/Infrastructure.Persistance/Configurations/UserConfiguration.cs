using InnoGotchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasOne(u => u.UserFarm)
                .WithOne(f => f.User)
                .HasForeignKey<Farm>(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(u => u.FriendsFarms)
                .WithMany(f => f.Collaborators);

            builder.Property(u => u.AvatarPath)
                .HasDefaultValue("default.jpg");
        }
    }
}
