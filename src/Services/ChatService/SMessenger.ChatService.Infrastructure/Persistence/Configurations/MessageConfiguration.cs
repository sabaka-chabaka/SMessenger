using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMessenger.ChatService.Domain.Entities;

namespace SMessenger.ChatService.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("MESSAGES");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("id");

        builder.Property(m => m.ChatId)
            .HasColumnName("chat_id")
            .IsRequired();

        builder.Property(m => m.SenderId)
            .HasColumnName("sender_id")
            .IsRequired();

        builder.Property(m => m.CiphertextBase64)
            .HasColumnName("ciphertext_base64")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(m => m.Nonce)
            .HasColumnName("nonce")
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(m => m.IsEdited)
            .HasColumnName("is_edited")
            .IsRequired();

        builder.Property(m => m.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired();

        builder.Property(m => m.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(m => m.EditedAt)
            .HasColumnName("edited_at");
        
        builder.HasOne(m => m.Chat)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatId);
    }
}
