using Microsoft.EntityFrameworkCore;
using PersonalDigitalVaultBackend.Models;

namespace PersonalDigitalVaultBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<FolderCategory> Folders => Set<FolderCategory>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FolderCategory>(entity =>
            {
                entity.HasOne(f => f.User)
                      .WithMany(u => u.Folders)
                      .HasForeignKey(f => f.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(f => f.ParentFolder)
                      .WithMany(f => f.SubFolders)
                      .HasForeignKey(f => f.ParentFolderId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

    }
}
