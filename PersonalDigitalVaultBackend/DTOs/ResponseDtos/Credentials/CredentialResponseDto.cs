namespace PersonalDigitalVaultBackend.DTOs.ResponseDtos.Credentials
{
    public class CredentialListItemResponseDto
    {

        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? AccountUsername { get; set; }
        public string? Url { get; set; }
        public int? FolderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string MaskedPassword { get; set; } = "********";
    }

    /// <summary>Only returned by the explicit "reveal" endpoint, decrypted on demand.</summary>
    public class CredentialRevealResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? AccountUsername { get; set; }
        public string Password { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? Url { get; set; }
    }
}
