using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Admin;

namespace PersonalDigitalVaultBackend.Services.Interface
{
    public interface IAdminService
    {
        Task<DashboardStatsResponseDto> GetDashboardStatsAsync();
        Task<List<AdminUserListItemResponseDto>> GetAllUsersAsync();
        Task SetUserActiveStatusAsync(int userId, bool isActive);
        Task DeleteUserAsync(int userId);
    }
}
