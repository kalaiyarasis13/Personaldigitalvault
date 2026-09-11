using PersonalDigitalVaultBackend.Models;

namespace PersonalDigitalVaultBackend.Repositories.Interfaces
{
    public interface ISharedLinkRepository
    {
        Task<SharedLink> AddAsync(SharedLink link);
        Task<SharedLink?> GetByTokenAsync(string token);
        Task<SharedLink?> GetByIdForUserAsync(int id, int userId);
        Task<List<SharedLink>> GetAllForUserAsync(int userId);
        Task UpdateAsync(SharedLink link);
    }
}
