using PersonalDigitalVaultBackend.DTOs.RequestDtos.Feedback;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Feedback;

namespace PersonalDigitalVaultBackend.Services.Interface
{
    public interface IFeedbackService
    {
        Task<FeedbackResponseDto> SubmitAsync(int userId, SubmitFeedbackRequestDto dto);
        Task<FeedbackResponseDto?> GetMyLatestAsync(int userId);
        Task<List<AdminFeedbackListItemResponseDto>> GetAllForAdminAsync();
    }
}
