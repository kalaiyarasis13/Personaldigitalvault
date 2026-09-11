using PersonalDigitalVaultBackend.Models;

namespace PersonalDigitalVaultBackend.Services.Interface
{
    public interface ITokenService
    {
        (string token, DateTime expiresAt) GenerateToken(ApplicationUser user);
    }
}
