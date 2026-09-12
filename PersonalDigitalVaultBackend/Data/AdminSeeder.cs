using Microsoft.AspNetCore.Identity;
using PersonalDigitalVaultBackend.Models;

namespace PersonalDigitalVaultBackend.Data
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(AppDbContext context, IConfiguration configuration)
        {
            await context.Database.EnsureCreatedAsync();

            bool adminExists = context.Users.Any(u => u.Role == UserRole.Administrator);
            if (adminExists) return;

            var username = configuration["AdminSeed:Username"] ?? "admin";
            var email = configuration["AdminSeed:Email"] ?? "admin@personaldigitalvault.local";
            var password = configuration["AdminSeed:Password"] ?? "Admin@12345";

            var hasher = new PasswordHasher<ApplicationUser>();
            var admin = new ApplicationUser
            {
                Username = username,
                Email = email,
                FullName = "System Administrator",
                Role = UserRole.Administrator,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            admin.PasswordHash = hasher.HashPassword(admin, password);

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
