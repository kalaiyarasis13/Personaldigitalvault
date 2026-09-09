namespace PersonalDigitalVaultBackend.DTOs.ResponseDtos.Admin
{
    public class DashboardStatsResponseDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalUploads { get; set; }
        public int TotalStoredFiles { get; set; }
        public long TotalStorageBytes { get; set; }
        public int TotalCredentialRecords { get; set; }
    }
    public class AdminUserListItemResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int DocumentCount { get; set; }
        public int FolderCount { get; set; }
    }

}
