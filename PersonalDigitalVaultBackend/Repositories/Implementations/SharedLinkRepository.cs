using Microsoft.EntityFrameworkCore;
using PersonalDigitalVaultBackend.Data;
using PersonalDigitalVaultBackend.Models;
using PersonalDigitalVaultBackend.Repositories.Interfaces;

namespace PersonalDigitalVaultBackend.Repositories.Implementations
{
    public class SharedLinkRepository : ISharedLinkRepository
    {
        private readonly AppDbContext _context;
        public SharedLinkRepository(AppDbContext context) => _context = context;

        public async Task<SharedLink> AddAsync(SharedLink link)
        {
            _context.SharedLinks.Add(link);
            await _context.SaveChangesAsync();
            return link;
        }

        public Task<SharedLink?> GetByTokenAsync(string token) =>
            _context.SharedLinks.Include(s => s.Document).FirstOrDefaultAsync(s => s.Token == token);

        public Task<SharedLink?> GetByIdForUserAsync(int id, int userId) =>
            _context.SharedLinks.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

        public Task<List<SharedLink>> GetAllForUserAsync(int userId) =>
            _context.SharedLinks
                .Include(s => s.Document)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

        public async Task UpdateAsync(SharedLink link)
        {
            _context.SharedLinks.Update(link);
            await _context.SaveChangesAsync();
        }
    }
}
