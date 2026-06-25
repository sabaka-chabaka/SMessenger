using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMessenger.UserService.Domain.Entities;

namespace SMessenger.UserService.Infrastructure.Persistence.Configurations;

public class UserBlockConfiguration : IEntityTypeConfiguration<UserBlock>
{
    public void Configure(EntityTypeBuilder<UserBlock> builder)
    {
        builder.ToTable("USER_BLOCKS");

        builder.HasKey(b => new { b.BlockerUserId, b.BlockedUserId });

        builder.Property(b => b.BlockerUserId)
            .HasColumnName("blocker_user_id");

        builder.Property(b => b.BlockedUserId)
            .HasColumnName("blocked_user_id");

        builder.Property(b => b.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        // No FK to UserProfile here: blocks can predate or outlive a profile row
        // in edge cases (e.g., profile deletion), and UserService doesn't need
        // cascade semantics for this — it's a flat relation table.
        builder.HasIndex(b => b.BlockerUserId);
    }
}