namespace PersonalDigitalVaultBackend.DTOs.ResponseDtos.Payment
{
    public class PaymentResultResponseDto
    {
        public bool Success { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string MockGatewayReference { get; set; } = string.Empty;
        public string CurrentPlan { get; set; } = string.Empty;
    }

    public class BillingStatusResponseDto
    {
        public string CurrentPlan { get; set; } = string.Empty;
        public long StorageUsedBytes { get; set; }
        public long StorageLimitBytes { get; set; }
    }

    public class PaymentHistoryItemResponseDto
    {
        public int Id { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public int AmountCents { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string MockGatewayReference { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

}

