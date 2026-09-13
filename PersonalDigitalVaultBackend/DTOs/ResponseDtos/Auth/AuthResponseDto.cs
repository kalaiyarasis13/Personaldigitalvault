namespace PersonalDigitalVaultBackend.DTOs.ResponseDtos.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserProfileResponseDto User { get; set; } = new();
    }
}
