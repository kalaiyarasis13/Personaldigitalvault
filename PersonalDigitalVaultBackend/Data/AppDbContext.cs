using Microsoft.EntityFrameworkCore;
using PersonalDigitalVaultBackend.Models;

namespace PersonalDigitalVaultBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CredentialRecord> Credentials => Set<CredentialRecord>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CredentialRecord>(entity =>
            {
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Credentials)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Folder)
                      .WithMany(f => f.Credentials)
                      .HasForeignKey(c => c.FolderId)
                      .OnDelete(DeleteBehavior.Restrict); 
            });
        }
    }
}
