using PersonalDigitalVaultBackend.DTOs.RequestDtos.Auth;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Auth;

namespace PersonalDigitalVaultBackend.Services.Interface
{
    public interface IAuService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
        Task<UserProfileResponseDto> GetProfileAsync(int userId);
        Task<UserProfileResponseDto> UpdateProfileAsync(int userId, UpdateProfileRequestDto dto);
        Task ChangePasswordAsync(int userId, ChangePasswordRequestDto dto);
    }
}
