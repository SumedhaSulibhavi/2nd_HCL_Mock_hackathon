using BCrypt.Net;
using HypeHealthAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace HypeHealthAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<DailyMoodLog> DailyMoodLogs { get; set; }
        public DbSet<KudosCard> KudosCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Enforce unique index constraints on email addresses
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // 2. Map structural business rule: One mood log entry per user per calendar day
            modelBuilder.Entity<DailyMoodLog>()
                .HasIndex(m => new { m.UserId, m.LogDate })
                .IsUnique();

            // 3. Configure column precision parameters on tracking metrics
            modelBuilder.Entity<User>()
                .Property(u => u.SprintVelocity)
                .HasColumnType("decimal(10,2)");

            // 4. Configure index configurations on foreign keys for query tracking optimization
            modelBuilder.Entity<DailyMoodLog>()
                .HasIndex(m => m.UserId)
                .HasDatabaseName("IX_DailyMoodLog_UserId");

            modelBuilder.Entity<KudosCard>()
                .HasIndex(k => k.SenderId)
                .HasDatabaseName("IX_KudosCard_SenderId");

            modelBuilder.Entity<KudosCard>()
                .HasIndex(k => k.ReceiverId)
                .HasDatabaseName("IX_KudosCard_ReceiverId");

            // 5. Restrict cascade delete operations on complex self-referential relations
            modelBuilder.Entity<KudosCard>()
                .HasOne(k => k.Sender)
                .WithMany()
                .HasForeignKey(k => k.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KudosCard>()
                .HasOne(k => k.Receiver)
                .WithMany()
                .HasForeignKey(k => k.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // 6. Seed Baseline Profiles (Passwords: Pass@123)
            string hashedPwd = BCrypt.Net.BCrypt.HashPassword("Pass@123");

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "Sanjana Patil", Email = "emp@company.com", PasswordHash = hashedPwd, Role = "Employee", SprintVelocity = 42.50m },
                new User { Id = 2, Username = "Alex Mercer", Email = "mgr@company.com", PasswordHash = hashedPwd, Role = "Manager", SprintVelocity = 0.00m },
                new User { Id = 3, Username = "David Miller", Email = "david@company.com", PasswordHash = hashedPwd, Role = "Employee", SprintVelocity = 38.20m },
                new User { Id = 4, Username = "Sarah Jenkins", Email = "sarah@company.com", PasswordHash = hashedPwd, Role = "Employee", SprintVelocity = 45.10m }
            );

            // 7. Seed Real-Time Application Activity Mock Logs
            DateTime today = DateTime.UtcNow.Date;
            modelBuilder.Entity<DailyMoodLog>().HasData(
                new DailyMoodLog { Id = 1, UserId = 1, Score = 8, LogDate = today },
                new DailyMoodLog { Id = 2, UserId = 3, Score = 5, LogDate = today },
                new DailyMoodLog { Id = 3, UserId = 4, Score = 9, LogDate = today },
                new DailyMoodLog { Id = 4, UserId = 1, Score = 7, LogDate = today.AddDays(-1) },
                new DailyMoodLog { Id = 5, UserId = 3, Score = 4, LogDate = today.AddDays(-1) },
                new DailyMoodLog { Id = 6, UserId = 4, Score = 8, LogDate = today.AddDays(-1) }
            );

            // 8. Seed Kudos Wall Initial Activity Stream
            modelBuilder.Entity<KudosCard>().HasData(
                new KudosCard { Id = 1, SenderId = 1, ReceiverId = 3, Message = "@David saved my deployment pipeline at midnight! Incredible work.", Timestamp = DateTime.UtcNow.AddHours(-5) },
                new KudosCard { Id = 2, SenderId = 3, ReceiverId = 4, Message = "@Sarah handled the enterprise API configuration perfectly. Thanks for unblocking!", Timestamp = DateTime.UtcNow.AddHours(-2) },
                new KudosCard { Id = 3, SenderId = 4, ReceiverId = 1, Message = "Shoutout to @Sanjana for completing the complex DB scheme mapping ahead of schedule!", Timestamp = DateTime.UtcNow.AddMinutes(-30) }
            );
        }
    }
}