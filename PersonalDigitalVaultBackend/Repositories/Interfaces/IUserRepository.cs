using PersonalDigitalVaultBackend.Models;

namespace PersonalDigitalVaultBackend.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(int id);
        Task<ApplicationUser?> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<ApplicationUser> AddAsync(ApplicationUser user);
        Task UpdateAsync(ApplicationUser user);
        Task<List<ApplicationUser>> GetAllAsync();
        Task<int> CountAsync();
        Task<int> CountActiveAsync();
        Task DeleteAsync(ApplicationUser user);
    }
}
