using PersonalDigitalVaultBackend.DTOs.RequestDtos.Sharing;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Sharing;
using PersonalDigitalVaultBackend.Models;
using PersonalDigitalVaultBackend.Repositories.Interfaces;
using PersonalDigitalVaultBackend.Services.Interface;

namespace PersonalDigitalVaultBackend.Services.Implementations
{
    public class SharingService : ISharingService
    {
        private readonly ISharedLinkRepository _sharedLinkRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IConfiguration _configuration;

        public SharingService(
            ISharedLinkRepository sharedLinkRepository,
            IDocumentRepository documentRepository,
            IEncryptionService encryptionService,
            IFileStorageService fileStorageService,
            IConfiguration configuration)
        {
            _sharedLinkRepository = sharedLinkRepository;
            _documentRepository = documentRepository;
            _encryptionService = encryptionService;
            _fileStorageService = fileStorageService;
            _configuration = configuration;
        }

        public async Task<ShareLinkResponseDto> CreateShareLinkAsync(int userId, int documentId, CreateShareLinkRequestDto dto)
        {
            var document = await _documentRepository.GetByIdForUserAsync(documentId, userId)
                ?? throw new KeyNotFoundException("Document not found.");

            var link = new SharedLink
            {
                Token = Guid.NewGuid().ToString("N"),
                DocumentId = document.Id,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(dto.ExpiryHours),
                IsRevoked = false
            };

            await _sharedLinkRepository.AddAsync(link);

            var publicBaseUrl = _configuration["PublicBaseUrl"] ?? "http://localhost:4200";

            return new ShareLinkResponseDto
            {
                Token = link.Token,
                ShareUrl = $"{publicBaseUrl.TrimEnd('/')}/share/{link.Token}",
                ExpiresAt = link.ExpiresAt
            };
        }

        public async Task<List<ShareLinkListItemResponseDto>> GetAllForUserAsync(int userId)
        {
            var links = await _sharedLinkRepository.GetAllForUserAsync(userId);
            return links.Select(l => new ShareLinkListItemResponseDto
            {
                Id = l.Id,
                Token = l.Token,
                DocumentId = l.DocumentId,
                DocumentFileName = l.Document?.OriginalFileName ?? "(deleted document)",
                ExpiresAt = l.ExpiresAt,
                IsRevoked = l.IsRevoked,
                DownloadCount = l.DownloadCount,
                CreatedAt = l.CreatedAt
            }).ToList();
        }

        public async Task RevokeAsync(int userId, int shareLinkId)
        {
            var link = await _sharedLinkRepository.GetByIdForUserAsync(shareLinkId, userId)
                ?? throw new KeyNotFoundException("Share link not found.");

            link.IsRevoked = true;
            await _sharedLinkRepository.UpdateAsync(link);
        }

        public async Task<(byte[] content, string fileName, string contentType)> ResolvePublicLinkAsync(string token)
        {
            var link = await _sharedLinkRepository.GetByTokenAsync(token)
                ?? throw new KeyNotFoundException("This share link is invalid.");

            if (link.IsRevoked)
                throw new InvalidOperationException("This share link has been revoked.");

            if (DateTime.UtcNow > link.ExpiresAt)
                throw new InvalidOperationException("This share link has expired.");

            var document = link.Document
                ?? await _documentRepository.GetByIdAsync(link.DocumentId)
                ?? throw new KeyNotFoundException("The shared document no longer exists.");

            var encryptedBytes = await _fileStorageService.ReadEncryptedFileAsync(document.StoredFileName, document.UserId);
            var decryptedBytes = _encryptionService.DecryptBytes(encryptedBytes);

            link.DownloadCount += 1;
            await _sharedLinkRepository.UpdateAsync(link);

            return (decryptedBytes, document.OriginalFileName, document.ContentType);
        }
    }
}
