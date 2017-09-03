using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OraChallenge.API.Models
{
    public partial class OraChallengeDBContext : DbContext
    {
        public virtual DbSet<MessageRecord> MessageRecord { get; set; }
        public virtual DbSet<User> User { get; set; }

        public OraChallengeDBContext(DbContextOptions<OraChallengeDBContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageRecord>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("date");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnName("message")
                    .HasMaxLength(100);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MessageRecord)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_UserMessage");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }
    }
}