

namespace PersonalDigitalVaultBackend.Models
{

    public enum UserRole
    {
        User = 0,
        Administrator = 1
    }

    public enum StoragePlan
    {
        Free = 0,
        Premium = 1
    }

    public class ApplicationUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Added for the (out-of-BRD-scope) mock payments feature ---
        public StoragePlan Plan { get; set; } = StoragePlan.Free;

        public ICollection<FolderCategory> Folders { get; set; } = new List<FolderCategory>();
        public ICollection<Documents> Documents { get; set; } = new List<Documents>();
        public ICollection<CredentialRecord> Credentials { get; set; } = new List<CredentialRecord>();
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
        public ICollection<SharedLink> SharedLinks { get; set; } = new List<SharedLink>();
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}
