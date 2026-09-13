using Microsoft.EntityFrameworkCore;
using PersonalDigitalVaultBackend.Data;
using PersonalDigitalVaultBackend.Models;
using PersonalDigitalVaultBackend.Repositories.Interfaces;

namespace PersonalDigitalVaultBackend.Repositories.Implementations
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly AppDbContext _context;
        public FeedbackRepository(AppDbContext context) => _context = context;

        public async Task<Feedback> AddAsync(Feedback feedback)
        {
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            return feedback;
        }

        public Task<Feedback?> GetLatestForUserAsync(int userId) =>
            _context.Feedbacks
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .FirstOrDefaultAsync();

        public Task<List<Feedback>> GetAllWithUserAsync() =>
            _context.Feedbacks
                .Include(f => f.User)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
    }
}
