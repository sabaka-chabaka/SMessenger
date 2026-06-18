using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMessenger.ChatService.Domain.Entities;

namespace SMessenger.ChatService.Infrastructure.Persistence.Configurations;

public class UserPublicKeyConfiguration : IEntityTypeConfiguration<UserPublicKey>
{
    public void Configure(EntityTypeBuilder<UserPublicKey> builder)
    {
        builder.ToTable("USER_PUBLIC_KEYS");

        builder.HasKey(upk => upk.UserId);

        builder.Property(upk => upk.UserId)
            .HasColumnName("user_id");

        builder.Property(upk => upk.PublicKeyBase64)
            .HasColumnName("public_key_base64")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(upk => upk.Algorithm)
            .HasColumnName("algorithm")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(upk => upk.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(upk => upk.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasMany(upk => upk.EncryptedKeys)
            .WithOne(ek => ek.UserPublicKey)
            .HasForeignKey(ek => ek.UserId);
    }
}
