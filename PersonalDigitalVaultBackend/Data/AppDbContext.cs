using Microsoft.EntityFrameworkCore;
using PersonalDigitalVaultBackend.Models;

namespace PersonalDigitalVaultBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<Feedback> Feedbacks => Set<Feedback>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.HasOne(f => f.User)
                      .WithMany(u => u.Feedbacks)
                      .HasForeignKey(f => f.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
