

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
        public ICollection<Documents> Documents { get; set; } = new List<Documents>();
        public ICollection<CredentialRecord> Credentials { get; set; } = new List<CredentialRecord>();
    }
}
