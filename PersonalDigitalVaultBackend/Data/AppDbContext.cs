using Microsoft.EntityFrameworkCore;
using PersonalDigitalVaultBackend.Models;

namespace PersonalDigitalVaultBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<SharedLink> SharedLinks => Set<SharedLink>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SharedLink>(entity =>
            {
                entity.HasIndex(s => s.Token).IsUnique();

                entity.HasOne(s => s.User)
                      .WithMany(u => u.SharedLinks)
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.Restrict); // avoids "multiple cascade paths" - cleanup happens via the Document cascade below

                entity.HasOne(s => s.Document)
                      .WithMany()
                      .HasForeignKey(s => s.DocumentId)
                      .OnDelete(DeleteBehavior.Cascade); // deleting a document (or its owning user, which cascades to documents) cleans up its share links
            });
        }
    }
}
