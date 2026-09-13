using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalDigitalVaultBackend.DTOs.RequestDtos.Sharing;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Common;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Sharing;
using PersonalDigitalVaultBackend.Services.Interface;
using System.Security.Claims;

namespace PersonalDigitalVaultBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SharingController : ControllerBase
{
    private readonly ISharingService _sharingService;
    public SharingController(ISharingService sharingService) => _sharingService = sharingService;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [Authorize]
    [HttpPost("/api/documents/{documentId:int}/share")]
    public async Task<ActionResult<ApiResponseDto<ShareLinkResponseDto>>> CreateLink(int documentId, CreateShareLinkRequestDto dto)
    {
        var result = await _sharingService.CreateShareLinkAsync(CurrentUserId, documentId, dto);
        return Ok(ApiResponseDto<ShareLinkResponseDto>.Ok(result, "Share link created."));
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<ShareLinkListItemResponseDto>>>> GetAll()
    {
        var result = await _sharingService.GetAllForUserAsync(CurrentUserId);
        return Ok(ApiResponseDto<List<ShareLinkListItemResponseDto>>.Ok(result));
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<object>>> Revoke(int id)
    {
        await _sharingService.RevokeAsync(CurrentUserId, id);
        return Ok(ApiResponseDto<object>.Ok(new { }, "Share link revoked."));
    }

    /// <summary>PUBLIC endpoint - no authentication. Anyone with a valid, unexpired token can download.</summary>
    [AllowAnonymous]
    [HttpGet("/api/share/{token}")]
    public async Task<IActionResult> DownloadShared(string token)
    {
        var (content, fileName, contentType) = await _sharingService.ResolvePublicLinkAsync(token);
        return File(content, contentType, fileName);
    }
}
