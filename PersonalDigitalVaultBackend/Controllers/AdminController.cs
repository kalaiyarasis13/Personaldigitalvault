using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Admin;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Common;
using PersonalDigitalVaultBackend.Services.Interface;

namespace PersonalDigitalVaultBackend.Controllers
{
    [Authorize(Roles = "Administrator")]
    [ApiController]
    [Route("api/[controller]")]

    public class AdminController : ControllerBase
    {

        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService) => _adminService = adminService;

        [HttpGet("dashboard")]
        public async Task<ActionResult<ApiResponseDto<DashboardStatsResponseDto>>> GetDashboard()
        {
            var result = await _adminService.GetDashboardStatsAsync();
            return Ok(ApiResponseDto<DashboardStatsResponseDto>.Ok(result));
        }

        [HttpGet("users")]
        public async Task<ActionResult<ApiResponseDto<List<AdminUserListItemResponseDto>>>> GetUsers()
        {
            var result = await _adminService.GetAllUsersAsync();
            return Ok(ApiResponseDto<List<AdminUserListItemResponseDto>>.Ok(result));
        }

        [HttpPut("users/{id:int}/status")]
        public async Task<ActionResult<ApiResponseDto<object>>> SetStatus(int id, [FromQuery] bool isActive)
        {
            await _adminService.SetUserActiveStatusAsync(id, isActive);
            return Ok(ApiResponseDto<object>.Ok(new { }, isActive ? "User enabled." : "User disabled."));
        }

        [HttpDelete("users/{id:int}")]
        public async Task<ActionResult<ApiResponseDto<object>>> DeleteUser(int id)
        {
            await _adminService.DeleteUserAsync(id);
            return Ok(ApiResponseDto<object>.Ok(new { }, "User deleted."));
        }
    }


}

