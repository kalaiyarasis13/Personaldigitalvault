using PersonalDigitalVaultBackend.Data;
using PersonalDigitalVaultBackend.Repositories.Interfaces;
using PersonalDigitalVaultBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace PersonalDigitalVaultBackend.Repositories.Implementations
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _context;
        public DocumentRepository(AppDbContext context) => _context = context;

        public Task<Documents?> GetByIdAsync(int id) =>
            _context.Documents.FirstOrDefaultAsync(d => d.Id == id);

        public Task<Documents?> GetByIdForUserAsync(int id, int userId) =>
            _context.Documents.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

        public Task<List<Documents>> GetAllForUserAsync(int userId, int? folderId, string? searchTerm)
        {
            var query = _context.Documents.Where(d => d.UserId == userId);

            if (folderId.HasValue)
                query = query.Where(d => d.FolderId == folderId.Value);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(d => d.OriginalFileName.ToLower().Contains(searchTerm.ToLower()));

            return query.OrderByDescending(d => d.UploadedAt).ToListAsync();
        }

        public async Task<Documents> AddAsync(Documents document)
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task UpdateAsync(Documents document)
        {
            _context.Documents.Update(document);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Documents document)
        {
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }

        public Task<int> CountAllAsync() => _context.Documents.CountAsync();

        public Task<long> SumSizeBytesAsync() =>
            _context.Documents.SumAsync(d => (long)d.SizeBytes);

        public Task<long> SumSizeBytesForUserAsync(int userId) =>
            _context.Documents.Where(d => d.UserId == userId).SumAsync(d => (long)d.SizeBytes);

        public async Task UnlinkFromFolderAsync(int folderId)
        {
            var documents = await _context.Documents.Where(d => d.FolderId == folderId).ToListAsync();
            foreach (var document in documents)
                document.FolderId = null;

            await _context.SaveChangesAsync();
        }
    }
}
