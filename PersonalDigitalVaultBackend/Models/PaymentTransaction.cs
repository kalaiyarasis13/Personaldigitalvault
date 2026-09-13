namespace PersonalDigitalVaultBackend.Models;
public enum PaymentStatus
{
    Success = 0,
    Failed = 1
}
public class PaymentTransaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public string PlanName { get; set; } = "Premium";
    public int AmountCents { get; set; } = 499; 
    public string Currency { get; set; } = "USD";
    public PaymentStatus Status { get; set; }

   
    public string MockGatewayReference { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
