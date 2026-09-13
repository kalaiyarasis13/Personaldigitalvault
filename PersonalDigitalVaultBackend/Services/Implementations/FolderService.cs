using PersonalDigitalVaultBackend.DTOs.RequestDtos.Folders;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Folders;
using PersonalDigitalVaultBackend.Models;
using PersonalDigitalVaultBackend.Repositories.Interfaces;
using PersonalDigitalVaultBackend.Services.Interface;

namespace PersonalDigitalVaultBackend.Services.Implementations
{
    public class FolderService : IFolderService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly ICredentialRepository _credentialRepository;
        public FolderService(
        IFolderRepository folderRepository,
        IDocumentRepository documentRepository,
        ICredentialRepository credentialRepository)
        {
            _folderRepository = folderRepository;
            _documentRepository = documentRepository;
            _credentialRepository = credentialRepository;
        }
        public async Task<List<FolderResponseDto>> GetAllAsync(int userId)
        {
            var folders = await _folderRepository.GetAllForUserAsync(userId);
            return folders.Select(ToDto).ToList();
        }
        public async Task<FolderResponseDto> CreateAsync(int userId, CreateFolderRequestDto dto)
        {
            if (dto.ParentFolderId.HasValue)
            {
                var parent = await _folderRepository.GetByIdForUserAsync(dto.ParentFolderId.Value, userId)
                    ?? throw new KeyNotFoundException("Parent folder not found.");
            }

            if (await _folderRepository.NameExistsForUserAsync(dto.Name, userId, dto.ParentFolderId))
                throw new InvalidOperationException("A folder with this name already exists here.");

            var folder = new FolderCategory
            {
                Name = dto.Name.Trim(),
                ParentFolderId = dto.ParentFolderId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _folderRepository.AddAsync(folder);
            return ToDto(folder);
        }
        public async Task<FolderResponseDto> RenameAsync(int userId, int folderId, RenameFolderRequestDto dto)
        {
            var folder = await _folderRepository.GetByIdForUserAsync(folderId, userId)
                ?? throw new KeyNotFoundException("Folder not found.");

            if (await _folderRepository.NameExistsForUserAsync(dto.Name, userId, folder.ParentFolderId, folderId))
                throw new InvalidOperationException("A folder with this name already exists here.");

            folder.Name = dto.Name.Trim();
            await _folderRepository.UpdateAsync(folder);
            return ToDto(folder);
        }
        public async Task DeleteAsync(int userId, int folderId)
        {
            var folder = await _folderRepository.GetByIdForUserAsync(folderId, userId)
                ?? throw new KeyNotFoundException("Folder not found.");

            var subFolderCount = await _folderRepository.CountSubFoldersAsync(folderId);
            if (subFolderCount > 0)
                throw new InvalidOperationException("Cannot delete a folder that contains sub-folders. Delete or move them first.");

            // The FolderId foreign keys on Document/Credential are Restrict (not SetNull) to avoid
            // SQL Server's "multiple cascade paths" error, so unlink them here in application code
            // before deleting the folder - this preserves the original "documents survive, just
            // become un-foldered" behavior.
            await _documentRepository.UnlinkFromFolderAsync(folderId);
            await _credentialRepository.UnlinkFromFolderAsync(folderId);

            await _folderRepository.DeleteAsync(folder);
        }
        private static FolderResponseDto ToDto(FolderCategory folder) => new()
        {
            Id = folder.Id,
            Name = folder.Name,
            ParentFolderId = folder.ParentFolderId,
            CreatedAt = folder.CreatedAt,
            DocumentCount = folder.Documents?.Count ?? 0,
            CredentialCount = folder.Credentials?.Count ?? 0,
            SubFolderCount = folder.SubFolders?.Count ?? 0
        };
    }
}
