using PersonalDigitalVaultBackend.Models;

namespace PersonalDigitalVaultBackend.Repositories.Interfaces
{
    public interface ICredentialRepository
    {

        Task<CredentialRecord?> GetByIdAsync(int id);
        Task<CredentialRecord?> GetByIdForUserAsync(int id, int userId);
        Task<List<CredentialRecord>> GetAllForUserAsync(int userId, int? folderId, string? searchTerm);
        Task<CredentialRecord> AddAsync(CredentialRecord credential);
        Task UpdateAsync(CredentialRecord credential);
        Task DeleteAsync(CredentialRecord credential);
        Task<int> CountAllAsync();

        /// <summary>Sets FolderId to null for every credential in the given folder - used before deleting a folder.</summary>
        Task UnlinkFromFolderAsync(int folderId);
    }
}
