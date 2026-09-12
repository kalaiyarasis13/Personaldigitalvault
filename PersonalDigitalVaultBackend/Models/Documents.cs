namespace PersonalDigitalVaultBackend.Models
{
    public class Documents
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/octet-stream";
        public long SizeBytes { get; set; }
        public string FileHash { get; set; } = string.Empty;

        public int? FolderId { get; set; }
        public FolderCategory? Folder { get; set; }

        public int UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
