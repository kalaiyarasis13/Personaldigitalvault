using PersonalDigitalVaultBackend.DTOs.RequestDtos.Credentials;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Credentials;
using PersonalDigitalVaultBackend.Models;
using PersonalDigitalVaultBackend.Repositories.Interfaces;
using PersonalDigitalVaultBackend.Services.Interface;

namespace PersonalDigitalVaultBackend.Services.Implementations
{
    public class CredentialService : ICredentialService
    {
        private readonly ICredentialRepository _credentialRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly IEncryptionService _encryptionService;

        public CredentialService(
            ICredentialRepository credentialRepository,
            IFolderRepository folderRepository,
            IEncryptionService encryptionService)
        {
            _credentialRepository = credentialRepository;
            _folderRepository = folderRepository;
            _encryptionService = encryptionService;
        }

        public async Task<List<CredentialListItemResponseDto>> GetAllAsync(int userId, int? folderId, string? search)
        {
            var credentials = await _credentialRepository.GetAllForUserAsync(userId, folderId, search);
            return credentials.Select(ToListDto).ToList();
        }

        public async Task<CredentialRevealResponseDto> RevealAsync(int userId, int credentialId)
        {
            var credential = await _credentialRepository.GetByIdForUserAsync(credentialId, userId)
                ?? throw new KeyNotFoundException("Credential record not found.");

            return new CredentialRevealResponseDto
            {
                Id = credential.Id,
                Title = credential.Title,
                AccountUsername = credential.AccountUsername,
                Password = _encryptionService.DecryptText(credential.EncryptedPassword),
                Notes = credential.EncryptedNotes is null ? null : _encryptionService.DecryptText(credential.EncryptedNotes),
                Url = credential.Url
            };
        }

        public async Task<CredentialListItemResponseDto> CreateAsync(int userId, CreateCredentialRequestDto dto)
        {
            if (dto.FolderId.HasValue)
            {
                _ = await _folderRepository.GetByIdForUserAsync(dto.FolderId.Value, userId)
                    ?? throw new KeyNotFoundException("Folder not found.");
            }

            var credential = new CredentialRecord
            {
                Title = dto.Title.Trim(),
                AccountUsername = dto.AccountUsername,
                EncryptedPassword = _encryptionService.EncryptText(dto.Password),
                EncryptedNotes = string.IsNullOrWhiteSpace(dto.Notes) ? null : _encryptionService.EncryptText(dto.Notes),
                Url = dto.Url,
                FolderId = dto.FolderId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _credentialRepository.AddAsync(credential);
            return ToListDto(credential);
        }

        public async Task<CredentialListItemResponseDto> UpdateAsync(int userId, int credentialId, UpdateCredentialRequestDto dto)
        {
            var credential = await _credentialRepository.GetByIdForUserAsync(credentialId, userId)
                ?? throw new KeyNotFoundException("Credential record not found.");

            if (dto.FolderId.HasValue)
            {
                _ = await _folderRepository.GetByIdForUserAsync(dto.FolderId.Value, userId)
                    ?? throw new KeyNotFoundException("Folder not found.");
            }

            credential.Title = dto.Title.Trim();
            credential.AccountUsername = dto.AccountUsername;
            credential.Url = dto.Url;
            credential.FolderId = dto.FolderId;
            credential.EncryptedNotes = string.IsNullOrWhiteSpace(dto.Notes) ? null : _encryptionService.EncryptText(dto.Notes);

            if (!string.IsNullOrWhiteSpace(dto.Password))
                credential.EncryptedPassword = _encryptionService.EncryptText(dto.Password);

            credential.UpdatedAt = DateTime.UtcNow;
            await _credentialRepository.UpdateAsync(credential);
            return ToListDto(credential);
        }

        public async Task DeleteAsync(int userId, int credentialId)
        {
            var credential = await _credentialRepository.GetByIdForUserAsync(credentialId, userId)
                ?? throw new KeyNotFoundException("Credential record not found.");

            await _credentialRepository.DeleteAsync(credential);
        }

        private static CredentialListItemResponseDto ToListDto(CredentialRecord credential) => new()
        {
            Id = credential.Id,
            Title = credential.Title,
            AccountUsername = credential.AccountUsername,
            Url = credential.Url,
            FolderId = credential.FolderId,
            CreatedAt = credential.CreatedAt,
            UpdatedAt = credential.UpdatedAt
        };
    }

}

