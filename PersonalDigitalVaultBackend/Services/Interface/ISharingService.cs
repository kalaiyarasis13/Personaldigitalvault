using PersonalDigitalVaultBackend.DTOs.RequestDtos.Sharing;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Sharing;

namespace PersonalDigitalVaultBackend.Services.Interface
{
    public interface ISharingService
    {
        Task<ShareLinkResponseDto> CreateShareLinkAsync(int userId, int documentId, CreateShareLinkRequestDto dto);
        Task<List<ShareLinkListItemResponseDto>> GetAllForUserAsync(int userId);
        Task RevokeAsync(int userId, int shareLinkId);

        /// <summary>Public, unauthenticated resolution of a share token to file bytes.</summary>
        Task<(byte[] content, string fileName, string contentType)> ResolvePublicLinkAsync(string token);
    }
}
