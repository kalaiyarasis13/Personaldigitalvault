using PersonalDigitalVaultBackend.Models;

namespace PersonalDigitalVaultBackend.Repositories.Interfaces
{
    public interface IDocumentRepository
    {
        Task<Documents?> GetByIdAsync(int id);
        Task<Documents?> GetByIdForUserAsync(int id, int userId);
        Task<List<Documents>> GetAllForUserAsync(int userId, int? folderId, string? searchTerm);
        Task<Documents> AddAsync(Documents document);
        Task UpdateAsync(Documents document);
        Task DeleteAsync(Documents document);
        Task<int> CountAllAsync();
        Task<long> SumSizeBytesAsync();
        Task<long> SumSizeBytesForUserAsync(int userId);

        /// <summary>Sets FolderId to null for every document in the given folder - used before deleting a folder.</summary>
        Task UnlinkFromFolderAsync(int folderId);
    }
}
