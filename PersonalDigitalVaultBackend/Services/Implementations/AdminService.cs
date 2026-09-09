using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Admin;
using PersonalDigitalVaultBackend.Repositories.Interfaces;
using PersonalDigitalVaultBackend.Services.Interface;

namespace PersonalDigitalVaultBackend.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly ICredentialRepository _credentialRepository;
        private readonly IFolderRepository _folderRepository;

        public AdminService(
            IUserRepository userRepository,
            IDocumentRepository documentRepository,
            ICredentialRepository credentialRepository,
            IFolderRepository folderRepository)
        {
            _userRepository = userRepository;
            _documentRepository = documentRepository;
            _credentialRepository = credentialRepository;
            _folderRepository = folderRepository;
        }

        public async Task<DashboardStatsResponseDto> GetDashboardStatsAsync() => new()
        {
            TotalUsers = await _userRepository.CountAsync(),
            ActiveUsers = await _userRepository.CountActiveAsync(),
            TotalUploads = await _documentRepository.CountAllAsync(),
            TotalStoredFiles = await _documentRepository.CountAllAsync(),
            TotalStorageBytes = await _documentRepository.SumSizeBytesAsync(),
            TotalCredentialRecords = await _credentialRepository.CountAllAsync()
        };

        public async Task<List<AdminUserListItemResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var result = new List<AdminUserListItemResponseDto>();

            foreach (var user in users)
            {
                var folders = await _folderRepository.GetAllForUserAsync(user.Id);
                result.Add(new AdminUserListItemResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    DocumentCount = user.Documents?.Count ?? 0,
                    FolderCount = folders.Count
                });
            }

            return result;
        }

        public async Task SetUserActiveStatusAsync(int userId, bool isActive)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            if (user.Role == UserRole.Administrator)
                throw new InvalidOperationException("Administrator accounts cannot be disabled.");

            user.IsActive = isActive;
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            if (user.Role == UserRole.Administrator)
                throw new InvalidOperationException("Administrator accounts cannot be deleted.");

            await _userRepository.DeleteAsync(user);
        }
    }
}
