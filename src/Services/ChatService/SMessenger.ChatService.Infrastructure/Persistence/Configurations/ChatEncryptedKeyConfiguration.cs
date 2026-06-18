using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMessenger.ChatService.Domain.Entities;

namespace SMessenger.ChatService.Infrastructure.Persistence.Configurations;

public class ChatEncryptedKeyConfiguration : IEntityTypeConfiguration<ChatEncryptedKey>
{
    public void Configure(EntityTypeBuilder<ChatEncryptedKey> builder)
    {
        builder.ToTable("CHAT_ENCRYPTED_KEYS");

        builder.HasKey(ek => new { ek.ChatId, ek.UserId });

        builder.Property(ek => ek.ChatId)
            .HasColumnName("chat_id");

        builder.Property(ek => ek.UserId)
            .HasColumnName("user_id");

        builder.Property(ek => ek.EncryptedKeyBase64)
            .HasColumnName("encrypted_key_base64")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(ek => ek.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasOne(ek => ek.Chat)
            .WithMany(c => c.EncryptedKeys)
            .HasForeignKey(ek => ek.ChatId);

        builder.HasOne(ek => ek.UserPublicKey)
            .WithMany(upk => upk.EncryptedKeys)
            .HasForeignKey(ek => ek.UserId);
    }
}
