using Microsoft.EntityFrameworkCore;
using PersonalDigitalVaultBackend.Data;
using PersonalDigitalVaultBackend.Models;
using PersonalDigitalVaultBackend.Repositories.Interfaces;

namespace PersonalDigitalVaultBackend.Repositories.Implementations
{
    public class CredentialRepository : ICredentialRepository
    {
        private readonly AppDbContext _context;
        public CredentialRepository(AppDbContext context) => _context = context;

        public Task<CredentialRecord?> GetByIdAsync(int id) =>
            _context.Credentials.FirstOrDefaultAsync(c => c.Id == id);

        public Task<CredentialRecord?> GetByIdForUserAsync(int id, int userId) =>
            _context.Credentials.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        public Task<List<CredentialRecord>> GetAllForUserAsync(int userId, int? folderId, string? searchTerm)
        {
            var query = _context.Credentials.Where(c => c.UserId == userId);

            if (folderId.HasValue)
                query = query.Where(c => c.FolderId == folderId.Value);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => c.Title.ToLower().Contains(searchTerm.ToLower()));

            return query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<CredentialRecord> AddAsync(CredentialRecord credential)
        {
            _context.Credentials.Add(credential);
            await _context.SaveChangesAsync();
            return credential;
        }

        public async Task UpdateAsync(CredentialRecord credential)
        {
            _context.Credentials.Update(credential);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CredentialRecord credential)
        {
            _context.Credentials.Remove(credential);
            await _context.SaveChangesAsync();
        }

        public Task<int> CountAllAsync() => _context.Credentials.CountAsync();

        public async Task UnlinkFromFolderAsync(int folderId)
        {
            var credentials = await _context.Credentials.Where(c => c.FolderId == folderId).ToListAsync();
            foreach (var credential in credentials)
                credential.FolderId = null;

            await _context.SaveChangesAsync();
        }
    }

}

