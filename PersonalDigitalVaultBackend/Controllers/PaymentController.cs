using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalDigitalVaultBackend.DTOs.RequestDtos.Payment;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Common;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Payment;
using PersonalDigitalVaultBackend.Services.Interface;
using System.Security.Claims;

namespace PersonalDigitalVaultBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService) => _paymentService = paymentService;


        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost("checkout")]
        public async Task<ActionResult<ApiResponseDto<PaymentResultResponseDto>>> Checkout(MockCheckoutRequestDto dto)
        {
            var result = await _paymentService.MockCheckoutAsync(CurrentUserId, dto);
            return Ok(ApiResponseDto<PaymentResultResponseDto>.Ok(result, "Payment successful! Plan upgraded."));
        }

        [HttpGet("billing-status")]
        public async Task<ActionResult<ApiResponseDto<BillingStatusResponseDto>>> BillingStatus()
        {
            var result = await _paymentService.GetBillingStatusAsync(CurrentUserId);
            return Ok(ApiResponseDto<BillingStatusResponseDto>.Ok(result));
        }

        [HttpGet("history")]
        public async Task<ActionResult<ApiResponseDto<List<PaymentHistoryItemResponseDto>>>> History()
        {
            var result = await _paymentService.GetHistoryAsync(CurrentUserId);
            return Ok(ApiResponseDto<List<PaymentHistoryItemResponseDto>>.Ok(result));
        }
    }
}
