using Microsoft.EntityFrameworkCore;
using PersonalDigitalVaultBackend.Data;
using PersonalDigitalVaultBackend.Models;
using PersonalDigitalVaultBackend.Repositories.Interfaces;

namespace PersonalDigitalVaultBackend.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) => _context = context;

        public Task<ApplicationUser?> GetByIdAsync(int id) =>
            _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        public Task<ApplicationUser?> GetByUsernameOrEmailAsync(string usernameOrEmail) =>
            _context.Users.FirstOrDefaultAsync(u =>
                u.Username == usernameOrEmail || u.Email == usernameOrEmail);

        public Task<bool> UsernameExistsAsync(string username) =>
            _context.Users.AnyAsync(u => u.Username == username);

        public Task<bool> EmailExistsAsync(string email) =>
            _context.Users.AnyAsync(u => u.Email == email);

        public async Task<ApplicationUser> AddAsync(ApplicationUser user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public Task<List<ApplicationUser>> GetAllAsync() =>
            _context.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();

        public Task<int> CountAsync() => _context.Users.CountAsync();

        public Task<int> CountActiveAsync() => _context.Users.CountAsync(u => u.IsActive);

        public async Task DeleteAsync(ApplicationUser user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
