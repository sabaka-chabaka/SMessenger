using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMessenger.ChatService.Domain.Entities;

namespace SMessenger.ChatService.Infrastructure.Persistence.Configurations;

public class ChatMemberConfiguration : IEntityTypeConfiguration<ChatMember>
{
    public void Configure(EntityTypeBuilder<ChatMember> builder)
    {
        builder.ToTable("CHAT_MEMBERS");

        builder.HasKey(cm => new { cm.ChatId, cm.UserId });

        builder.Property(cm => cm.ChatId)
            .HasColumnName("chat_id");

        builder.Property(cm => cm.UserId)
            .HasColumnName("user_id");

        builder.Property(cm => cm.Role)
            .HasColumnName("role")
            .HasMaxLength(20)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(cm => cm.JoinedAt)
            .HasColumnName("joined_at")
            .IsRequired();

        builder.Property(cm => cm.LastReadAt)
            .HasColumnName("last_read_at");

        builder.HasOne(cm => cm.Chat)
            .WithMany(c => c.Members)
            .HasForeignKey(cm => cm.ChatId);
    }
}
