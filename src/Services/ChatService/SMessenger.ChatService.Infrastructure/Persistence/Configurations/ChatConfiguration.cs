using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMessenger.ChatService.Domain.Entities;

namespace SMessenger.ChatService.Infrastructure.Persistence.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.ToTable("CHATS");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id");

        builder.Property(c => c.Type)
            .HasColumnName("type")
            .HasMaxLength(20)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasMany(c => c.Messages)
            .WithOne(m => m.Chat)
            .HasForeignKey(m => m.ChatId);

        builder.HasMany(c => c.Members)
            .WithOne(m => m.Chat)
            .HasForeignKey(m => m.ChatId);

        builder.HasMany(c => c.EncryptedKeys)
            .WithOne(k => k.Chat)
            .HasForeignKey(k => k.ChatId);
    }
}
