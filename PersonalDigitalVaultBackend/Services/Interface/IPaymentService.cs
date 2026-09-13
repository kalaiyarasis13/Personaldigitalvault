using PersonalDigitalVaultBackend.DTOs.RequestDtos.Payment;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Payment;

namespace PersonalDigitalVaultBackend.Services.Interface
{
    public interface IPaymentService
    {
        Task<PaymentResultResponseDto> MockCheckoutAsync(int userId, MockCheckoutRequestDto dto);
        Task<BillingStatusResponseDto> GetBillingStatusAsync(int userId);
        Task<List<PaymentHistoryItemResponseDto>> GetHistoryAsync(int userId);
    }
}
