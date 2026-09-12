using System.ComponentModel.DataAnnotations;

namespace PersonalDigitalVaultBackend.DTOs.RequestDtos.Documents
{
    public class RenameDocumentRequestDto
    {
        [Required, MaxLength(255)]
        public string NewFileName { get; set; } = string.Empty;
    }

    public class MoveDocumentRequestDto
    {
        public int? FolderId { get; set; }
    }
}
