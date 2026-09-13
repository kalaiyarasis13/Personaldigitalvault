using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalDigitalVaultBackend.Controllers;
using PersonalDigitalVaultBackend.DTOs.RequestDtos.Feedback;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Common;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Feedback;
using PersonalDigitalVaultBackend.Services.Interface;
using System.Security.Claims;


namespace PersonalDigitalVault.Controllers;

/// <summary>Post-checkout customer feedback. Not part of the original BRD; added on request.</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _feedbackService;
    public FeedbackController(IFeedbackService feedbackService) => _feedbackService = feedbackService;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<FeedbackResponseDto>>> Submit(SubmitFeedbackRequestDto dto)
    {
        var result = await _feedbackService.SubmitAsync(CurrentUserId, dto);
        return Ok(ApiResponseDto<FeedbackResponseDto>.Ok(result, "Thanks for your feedback!"));
    }

    [HttpGet("mine")]
    public async Task<ActionResult<ApiResponseDto<FeedbackResponseDto?>>> GetMine()
    {
        var result = await _feedbackService.GetMyLatestAsync(CurrentUserId);
        return Ok(ApiResponseDto<FeedbackResponseDto?>.Ok(result));
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<AdminFeedbackListItemResponseDto>>>> GetAll()
    {
        var result = await _feedbackService.GetAllForAdminAsync();
        return Ok(ApiResponseDto<List<AdminFeedbackListItemResponseDto>>.Ok(result));
    }
}

