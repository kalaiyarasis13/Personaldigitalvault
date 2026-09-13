using PersonalDigitalVaultBackend.DTOs.RequestDtos.Folders;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Folders;

namespace PersonalDigitalVaultBackend.Services.Interface
{
    public interface IFolderService
    {
        Task<List<FolderResponseDto>> GetAllAsync(int userId);
        Task<FolderResponseDto> CreateAsync(int userId, CreateFolderRequestDto dto);
        Task<FolderResponseDto> RenameAsync(int userId, int folderId, RenameFolderRequestDto dto);
        Task DeleteAsync(int userId, int folderId);
    }
}
