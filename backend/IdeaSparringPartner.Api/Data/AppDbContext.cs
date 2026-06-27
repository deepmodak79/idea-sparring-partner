using IdeaSparringPartner.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IdeaSparringPartner.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Idea> Ideas => Set<Idea>();
    public DbSet<Models.Thread> Threads => Set<Models.Thread>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Memory> Memories => Set<Memory>();
    public DbSet<Synthesis> Syntheses => Set<Synthesis>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.DisplayName).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TokenHash);
            entity.HasIndex(e => e.UserId);
            entity.Property(e => e.TokenHash).HasMaxLength(255).IsRequired();
            entity.HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Idea>(entity =>
        {
            entity.ToTable("ideas");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.CreatedAt });
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.HasOne(e => e.User)
                .WithMany(u => u.Ideas)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Models.Thread>(entity =>
        {
            entity.ToTable("threads");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.IdeaId, e.Persona }).IsUnique();
            entity.HasOne(e => e.Idea)
                .WithMany(i => i.Threads)
                .HasForeignKey(e => e.IdeaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("messages");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ThreadId, e.CreatedAt });
            entity.Property(e => e.Content).IsRequired();
            entity.HasOne(e => e.Thread)
                .WithMany(t => t.Messages)
                .HasForeignKey(e => e.ThreadId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Memory>(entity =>
        {
            entity.ToTable("memories");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.Scope, e.IsDeleted });
            entity.HasIndex(e => new { e.IdeaId, e.IsDeleted });
            entity.Property(e => e.Content).IsRequired();
            entity.HasOne(e => e.User)
                .WithMany(u => u.Memories)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Idea)
                .WithMany(i => i.Memories)
                .HasForeignKey(e => e.IdeaId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.SourceThread)
                .WithMany()
                .HasForeignKey(e => e.SourceThreadId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.SourceMessage)
                .WithMany()
                .HasForeignKey(e => e.SourceMessageId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Synthesis>(entity =>
        {
            entity.ToTable("syntheses");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.IdeaId, e.Version }).IsUnique();
            entity.Property(e => e.StrongestChallengesJson).IsRequired();
            entity.Property(e => e.WeakestReasoningJson).IsRequired();
            entity.Property(e => e.UnresolvedTensionsJson).IsRequired();
            entity.HasOne(e => e.Idea)
                .WithMany(i => i.Syntheses)
                .HasForeignKey(e => e.IdeaId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
