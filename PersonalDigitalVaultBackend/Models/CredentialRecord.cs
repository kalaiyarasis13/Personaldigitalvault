namespace PersonalDigitalVaultBackend.Models
{
    public class CredentialRecord
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? AccountUsername { get; set; }

        public string EncryptedPassword { get; set; } = string.Empty;
        public string? EncryptedNotes { get; set; }

        public string? Url { get; set; }

        public int? FolderId { get; set; }
        public FolderCategory? Folder { get; set; }

        public int UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
