using Microsoft.EntityFrameworkCore;
using PersonalDigitalVaultBackend.Data;
using PersonalDigitalVaultBackend.Models;
using PersonalDigitalVaultBackend.Repositories.Interfaces;

namespace PersonalDigitalVaultBackend.Repositories.Implementations
{
    public class FolderRepository : IFolderRepository
    {
        private readonly AppDbContext _context;
        public FolderRepository(AppDbContext context) => _context = context;

        public Task<FolderCategory?> GetByIdAsync(int id) =>
            _context.Folders.FirstOrDefaultAsync(f => f.Id == id);

        public Task<FolderCategory?> GetByIdForUserAsync(int id, int userId) =>
            _context.Folders.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

        public Task<List<FolderCategory>> GetAllForUserAsync(int userId) =>
            _context.Folders
                .Where(f => f.UserId == userId)
                .Include(f => f.Documents)
                .Include(f => f.Credentials)
                .Include(f => f.SubFolders)
                .OrderBy(f => f.Name)
                .ToListAsync();

        public async Task<FolderCategory> AddAsync(FolderCategory folder)
        {
            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();
            return folder;
        }

        public async Task UpdateAsync(FolderCategory folder)
        {
            _context.Folders.Update(folder);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(FolderCategory folder)
        {
            _context.Folders.Remove(folder);
            await _context.SaveChangesAsync();
        }

        public Task<bool> NameExistsForUserAsync(string name, int userId, int? parentFolderId, int? excludeFolderId = null)
        {
            var query = _context.Folders.Where(f =>
                f.UserId == userId &&
                f.Name.ToLower() == name.ToLower() &&
                f.ParentFolderId == parentFolderId);

            if (excludeFolderId.HasValue)
                query = query.Where(f => f.Id != excludeFolderId.Value);

            return query.AnyAsync();
        }

        public Task<int> CountSubFoldersAsync(int folderId) =>
            _context.Folders.CountAsync(f => f.ParentFolderId == folderId);
    }
}
