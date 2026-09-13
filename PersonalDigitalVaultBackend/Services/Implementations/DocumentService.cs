using PersonalDigitalVaultBackend.DTOs.RequestDtos.Documents;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Documents;
using PersonalDigitalVaultBackend.Repositories.Interfaces;
using PersonalDigitalVaultBackend.Services.Interface;
using PersonalDigitalVaultBackend.Models;

namespace PersonalDigitalVaultBackend.Services.Implementations
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IFileStorageService _fileStorageService;

        // Keep the demo footprint sane - 25 MB per file.
        private const long MaxFileSizeBytes = 25 * 1024 * 1024;

        private static readonly HashSet<string> BlockedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".exe", ".dll", ".bat", ".cmd", ".sh", ".msi"
    };

        public DocumentService(
            IDocumentRepository documentRepository,
            IFolderRepository folderRepository,
            IEncryptionService encryptionService,
            IFileStorageService fileStorageService)
        {
            _documentRepository = documentRepository;
            _folderRepository = folderRepository;
            _encryptionService = encryptionService;
            _fileStorageService = fileStorageService;
        }

        public async Task<List<DocumentResponesDto>> GetAllAsync(int userId, int? folderId, string? search)
        {
            var documents = await _documentRepository.GetAllForUserAsync(userId, folderId, search);
            return documents.Select(ToDto).ToList();
        }

        public async Task<DocumentResponesDto> UploadAsync(int userId, IFormFile file, int? folderId)
        {
            if (file is null || file.Length == 0)
                throw new InvalidOperationException("No file was uploaded.");

            if (file.Length > MaxFileSizeBytes)
                throw new InvalidOperationException("File exceeds the 25 MB upload limit.");

            var extension = Path.GetExtension(file.FileName);
            if (BlockedExtensions.Contains(extension))
                throw new InvalidOperationException($"Files with extension {extension} are not allowed.");

            if (folderId.HasValue)
            {
                _ = await _folderRepository.GetByIdForUserAsync(folderId.Value, userId)
                    ?? throw new KeyNotFoundException("Folder not found.");
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var originalBytes = memoryStream.ToArray();

            // 1. Compute the integrity hash BEFORE encrypting (hash of the original content).
            var fileHash = _encryptionService.ComputeSha256(originalBytes);

            // 2. Encrypt, then persist to the protected folder outside wwwroot.
            var encryptedBytes = _encryptionService.EncryptBytes(originalBytes);
            var storedFileName = await _fileStorageService.SaveEncryptedFileAsync(encryptedBytes, file.FileName, userId);

            var document = new Documents
            {
                OriginalFileName = file.FileName,
                StoredFileName = storedFileName,
                ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                SizeBytes = file.Length,
                FileHash = fileHash,
                FolderId = folderId,
                UserId = userId,
                UploadedAt = DateTime.UtcNow
            };

            await _documentRepository.AddAsync(document);
            return ToDto(document);
        }

        public async Task<(byte[] content, string fileName, string contentType)> DownloadAsync(int userId, int documentId)
        {
            var document = await _documentRepository.GetByIdForUserAsync(documentId, userId)
                ?? throw new KeyNotFoundException("Document not found.");

            var encryptedBytes = await _fileStorageService.ReadEncryptedFileAsync(document.StoredFileName, userId);
            var decryptedBytes = _encryptionService.DecryptBytes(encryptedBytes);

            // Verify integrity on every download - protects against silent tampering/corruption on disk.
            var currentHash = _encryptionService.ComputeSha256(decryptedBytes);
            if (currentHash != document.FileHash)
                throw new InvalidOperationException("File integrity check failed - the stored file may have been altered.");

            return (decryptedBytes, document.OriginalFileName, document.ContentType);
        }

        public async Task<DocumentResponesDto> RenameAsync(int userId, int documentId, RenameDocumentRequestDto dto)
        {
            var document = await _documentRepository.GetByIdForUserAsync(documentId, userId)
                ?? throw new KeyNotFoundException("Document not found.");

            document.OriginalFileName = dto.NewFileName.Trim();
            document.UpdatedAt = DateTime.UtcNow;
            await _documentRepository.UpdateAsync(document);
            return ToDto(document);
        }

        public async Task<DocumentResponesDto> MoveAsync(int userId, int documentId, MoveDocumentRequestDto dto)
        {
            var document = await _documentRepository.GetByIdForUserAsync(documentId, userId)
                ?? throw new KeyNotFoundException("Document not found.");

            if (dto.FolderId.HasValue)
            {
                _ = await _folderRepository.GetByIdForUserAsync(dto.FolderId.Value, userId)
                    ?? throw new KeyNotFoundException("Target folder not found.");
            }

            document.FolderId = dto.FolderId;
            document.UpdatedAt = DateTime.UtcNow;
            await _documentRepository.UpdateAsync(document);
            return ToDto(document);
        }

        public async Task DeleteAsync(int userId, int documentId)
        {
            var document = await _documentRepository.GetByIdForUserAsync(documentId, userId)
                ?? throw new KeyNotFoundException("Document not found.");

            _fileStorageService.DeleteFile(document.StoredFileName, userId);
            await _documentRepository.DeleteAsync(document);
        }

        private static DocumentResponesDto ToDto(Documents document) => new()
        {
            Id = document.Id,
            OriginalFileName = document.OriginalFileName,
            ContentType = document.ContentType,
            SizeBytes = document.SizeBytes,
            FileHash = document.FileHash,
            FolderId = document.FolderId,
            UploadedAt = document.UploadedAt,
            UpdatedAt = document.UpdatedAt
        };
    }
}
