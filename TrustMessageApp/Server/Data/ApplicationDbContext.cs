using Microsoft.EntityFrameworkCore;
using BaseLibrary.Entities;

namespace Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Username).HasMaxLength(50);
                entity.Property(u => u.Email).HasMaxLength(255);
                entity.Property(u => u.PublicKey).IsRequired();
                entity.Property(u => u.PrivateKey).IsRequired();
                entity.Property(u => u.TwoFactorSecret).IsRequired();
                entity.Property(u => u.FailedLoginAttempts).HasDefaultValue(0);
                entity.Property(u => u.LockoutEnd).HasDefaultValue(null);
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.HasOne(m => m.User)
                      .WithMany()
                      .HasForeignKey(m => m.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(m => m.Content).IsRequired();
                entity.Property(m => m.Signature).IsRequired();
                entity.Property(m => m.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}