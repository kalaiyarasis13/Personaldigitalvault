using System.Reflection.Metadata;

namespace PersonalDigitalVaultBackend.Models
{
    public class FolderCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentFolderId { get; set; }
        public FolderCategory? ParentFolder { get; set; }

        public int UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<FolderCategory> SubFolders { get; set; } = new List<FolderCategory>();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public ICollection<CredentialRecord> Credentials { get; set; } = new List<CredentialRecord>();
    }
}
