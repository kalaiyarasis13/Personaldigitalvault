using Microsoft.EntityFrameworkCore;
using PersonalDigitalVaultBackend.Models;


namespace PersonalDigitalVaultBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<SharedLink> SharedLinks => Set<SharedLink>();
        public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
        public DbSet<FolderCategory> Folders => Set<FolderCategory>();
        public DbSet<Documents> Documents => Set<Documents>();
        public DbSet<Feedback> Feedbacks => Set<Feedback>();
        public DbSet<CredentialRecord> Credentials => Set<CredentialRecord>();
        public DbSet<ApplicationUser> Users => Set<ApplicationUser>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Documents>(entity =>
            {
                entity.HasOne(d => d.User)
                      .WithMany(u => u.Documents)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Folder)
                      .WithMany(f => f.Documents)
                      .HasForeignKey(d => d.FolderId)
                      .OnDelete(DeleteBehavior.Restrict); // avoids "multiple cascade paths" - app code unlinks documents before a folder is deleted
            });

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
            modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.HasOne(p => p.User)
                      .WithMany(u => u.PaymentTransactions)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
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
            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.HasOne(f => f.User)
                      .WithMany(u => u.Feedbacks)
                      .HasForeignKey(f => f.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
             
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
             
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
            });
        }

    }
}
