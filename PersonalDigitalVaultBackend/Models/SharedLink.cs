using System.Reflection.Metadata;

namespace PersonalDigitalVaultBackend.Models;

public class SharedLink
{
    public int Id { get; set; }
    public string Token { get; set; } = Guid.NewGuid().ToString("N");

    public int DocumentId { get; set; }
    public Document? Document { get; set; }

    public int UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public int DownloadCount { get; set; } = 0;
}
