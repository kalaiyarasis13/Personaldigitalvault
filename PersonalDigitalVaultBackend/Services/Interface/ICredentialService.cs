using PersonalDigitalVaultBackend.DTOs.RequestDtos.Credentials;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Credentials;

namespace PersonalDigitalVaultBackend.Services.Interface
{
    public interface ICredentialService
    {
        Task<List<CredentialListItemResponseDto>> GetAllAsync(int userId, int? folderId, string? search);
        Task<CredentialRevealResponseDto> RevealAsync(int userId, int credentialId);
        Task<CredentialListItemResponseDto> CreateAsync(int userId, CreateCredentialRequestDto dto);
        Task<CredentialListItemResponseDto> UpdateAsync(int userId, int credentialId, UpdateCredentialRequestDto dto);
        Task DeleteAsync(int userId, int credentialId);
    }
}
