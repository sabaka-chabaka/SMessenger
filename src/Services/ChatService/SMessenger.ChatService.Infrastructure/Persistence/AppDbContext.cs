using Microsoft.EntityFrameworkCore;
using SMessenger.ChatService.Domain.Entities;

namespace SMessenger.ChatService.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ChatMember> ChatMembers { get; set; }
    public DbSet<UserPublicKey> UserPublicKeys { get; set; }
    public DbSet<ChatEncryptedKey> ChatEncryptedKeys { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}