using PersonalDigitalVaultBackend.Controllers;
using PersonalDigitalVaultBackend.DTOs.RequestDtos.Feedback;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Feedback;
using PersonalDigitalVaultBackend.Repositories.Interfaces;
using PersonalDigitalVaultBackend.Services.Interface;
using FeedbackModel = PersonalDigitalVaultBackend.Models.Feedback;

namespace PersonalDigitalVault.Services.Implementations;

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _feedbackRepository;
    public FeedbackService(IFeedbackRepository feedbackRepository) => _feedbackRepository = feedbackRepository;

    public async Task<FeedbackResponseDto> SubmitAsync(int userId, SubmitFeedbackRequestDto dto)
    {
        var feedback = new FeedbackModel
        {
            UserId = userId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _feedbackRepository.AddAsync(feedback);

        return new FeedbackResponseDto
        {
            Id = feedback.Id,
            Rating = feedback.Rating,
            Comment = feedback.Comment,
            CreatedAt = feedback.CreatedAt
        };
    }

    public async Task<FeedbackResponseDto?> GetMyLatestAsync(int userId)
    {
        var feedback = await _feedbackRepository.GetLatestForUserAsync(userId);
        if (feedback is null) return null;

        return new FeedbackResponseDto
        {
            Id = feedback.Id,
            Rating = feedback.Rating,
            Comment = feedback.Comment,
            CreatedAt = feedback.CreatedAt
        };
    }

    public async Task<List<AdminFeedbackListItemResponseDto>> GetAllForAdminAsync()
    {
        var feedbacks = await _feedbackRepository.GetAllWithUserAsync();
        return feedbacks.Select(f => new AdminFeedbackListItemResponseDto
        {
            Id = f.Id,
            Username = f.User?.Username ?? "(deleted user)",
            Rating = f.Rating,
            Comment = f.Comment,
            CreatedAt = f.CreatedAt
        }).ToList();
    }
}
