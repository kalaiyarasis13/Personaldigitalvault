using System.ComponentModel.DataAnnotations;

namespace PersonalDigitalVaultBackend.DTOs.RequestDtos.Sharing
{
    public class CreateShareLinkRequestDto
    {
        [Range(1, 24 * 30)] // 1 hour to 30 days
        public int ExpiryHours { get; set; } = 24;
    }
}
