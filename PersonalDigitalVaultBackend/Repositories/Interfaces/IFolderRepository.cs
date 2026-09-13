using PersonalDigitalVaultBackend.Models;

namespace PersonalDigitalVaultBackend.Repositories.Interfaces
{
    public interface IFolderRepository
    {
        Task<FolderCategory?> GetByIdAsync(int id);
        Task<FolderCategory?> GetByIdForUserAsync(int id, int userId);
        Task<List<FolderCategory>> GetAllForUserAsync(int userId);
        Task<FolderCategory> AddAsync(FolderCategory folder);
        Task UpdateAsync(FolderCategory folder);
        Task DeleteAsync(FolderCategory folder);
        Task<bool> NameExistsForUserAsync(string name, int userId, int? parentFolderId, int? excludeFolderId = null);
        Task<int> CountSubFoldersAsync(int folderId);
    }
}
