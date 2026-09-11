using PersonalDigitalVaultBackend.Models;

namespace PersonalDigitalVaultBackend.Repositories.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<Feedback> AddAsync(Feedback feedback);
        Task<Feedback?> GetLatestForUserAsync(int userId);
        Task<List<Feedback>> GetAllWithUserAsync();
    }
}
