using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMessenger.UserService.Domain.Entities;

namespace SMessenger.UserService.Infrastructure.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("USER_PROFILES");

        builder.HasKey(p => p.UserId);

        builder.Property(p => p.UserId)
            .HasColumnName("user_id");

        builder.Property(p => p.Username)
            .HasColumnName("username")
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(p => p.Username)
            .IsUnique();

        builder.Property(p => p.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(p => p.AvatarUrl)
            .HasColumnName("avatar_url")
            .HasMaxLength(512);

        builder.Property(p => p.Bio)
            .HasColumnName("bio")
            .HasMaxLength(512);

        builder.Property(p => p.LastSeenAt)
            .HasColumnName("last_seen_at");

        builder.Property(p => p.ShowLastSeen)
            .HasColumnName("show_last_seen")
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}