using PersonalDigitalVaultBackend.DTOs.RequestDtos.Documents;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Documents;

namespace PersonalDigitalVaultBackend.Services.Interface
{
    public interface IDocumentService
    {
        Task<List<DocumentResponesDto>> GetAllAsync(int userId, int? folderId, string? search);
        Task<DocumentResponesDto> UploadAsync(int userId, IFormFile file, int? folderId);
        Task<(byte[] content, string fileName, string contentType)> DownloadAsync(int userId, int documentId);
        Task<DocumentResponesDto> RenameAsync(int userId, int documentId, RenameDocumentRequestDto dto);
        Task<DocumentResponesDto> MoveAsync(int userId, int documentId, MoveDocumentRequestDto dto);
        Task DeleteAsync(int userId, int documentId);
    }
}
