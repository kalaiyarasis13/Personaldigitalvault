using Microsoft.EntityFrameworkCore;
using PersonalDigitalVaultBackend.Models;

namespace PersonalDigitalVaultBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.HasOne(p => p.User)
                      .WithMany(u => u.PaymentTransactions)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
