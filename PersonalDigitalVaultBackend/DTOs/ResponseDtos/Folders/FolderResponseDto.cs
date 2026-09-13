namespace PersonalDigitalVaultBackend.DTOs.ResponseDtos.Folders
{
    public class FolderResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentFolderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int DocumentCount { get; set; }
        public int CredentialCount { get; set; }
        public int SubFolderCount { get; set; }
    }
}
