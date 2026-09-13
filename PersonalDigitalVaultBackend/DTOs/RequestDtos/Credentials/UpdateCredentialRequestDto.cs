using System.ComponentModel.DataAnnotations;

namespace PersonalDigitalVaultBackend.DTOs.RequestDtos.Credentials
{
    public class UpdateCredentialRequestDto
    {
        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? AccountUsername { get; set; }

        // If null/empty, password stays unchanged
        public string? Password { get; set; }

        public string? Notes { get; set; }

        [MaxLength(500)]
        public string? Url { get; set; }

        public int? FolderId { get; set; }
    }
}
