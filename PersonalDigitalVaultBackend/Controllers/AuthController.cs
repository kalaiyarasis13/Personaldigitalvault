using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalDigitalVaultBackend.DTOs.RequestDtos.Auth;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Auth;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Common;
using PersonalDigitalVaultBackend.Services.Interface;
using System.Security.Claims;

namespace PersonalDigitalVaultBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuService _authService;
        public AuthController(IAuService authService) => _authService = authService;

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponseDto<AuthResponseDto>>> Register(RegisterRequestDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(ApiResponseDto<AuthResponseDto>.Ok(result, "Account created successfully."));
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponseDto<AuthResponseDto>>> Login(LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(ApiResponseDto<AuthResponseDto>.Ok(result, "Login successful."));
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponseDto<UserProfileResponseDto>>> GetProfile()
        {
            var result = await _authService.GetProfileAsync(CurrentUserId);
            return Ok(ApiResponseDto<UserProfileResponseDto>.Ok(result));
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<ActionResult<ApiResponseDto<UserProfileResponseDto>>> UpdateProfile(UpdateProfileRequestDto dto)
        {
            var result = await _authService.UpdateProfileAsync(CurrentUserId, dto);
            return Ok(ApiResponseDto<UserProfileResponseDto>.Ok(result, "Profile updated."));
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<ActionResult<ApiResponseDto<object>>> ChangePassword(ChangePasswordRequestDto dto)
        {
            await _authService.ChangePasswordAsync(CurrentUserId, dto);
            return Ok(ApiResponseDto<object>.Ok(new { }, "Password changed successfully."));
        }
    }
}
