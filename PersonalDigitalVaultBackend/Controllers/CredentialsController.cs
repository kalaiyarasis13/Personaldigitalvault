using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalDigitalVaultBackend.DTOs.RequestDtos.Credentials;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Common;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Credentials;
using PersonalDigitalVaultBackend.Services.Interface;
using System.Security.Claims;

namespace PersonalDigitalVaultBackend.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class CredentialsController : ControllerBase
    {
        private readonly ICredentialService _credentialService;
        public CredentialsController(ICredentialService credentialService) => _credentialService = credentialService;

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<List<CredentialListItemResponseDto>>>> GetAll(
            [FromQuery] int? folderId, [FromQuery] string? search)
        {
            var result = await _credentialService.GetAllAsync(CurrentUserId, folderId, search);
            return Ok(ApiResponseDto<List<CredentialListItemResponseDto>>.Ok(result));
        }

        [HttpGet("{id:int}/reveal")]
        public async Task<ActionResult<ApiResponseDto<CredentialRevealResponseDto>>> Reveal(int id)
        {
            var result = await _credentialService.RevealAsync(CurrentUserId, id);
            return Ok(ApiResponseDto<CredentialRevealResponseDto>.Ok(result));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<CredentialListItemResponseDto>>> Create(CreateCredentialRequestDto dto)
        {
            var result = await _credentialService.CreateAsync(CurrentUserId, dto);
            return Ok(ApiResponseDto<CredentialListItemResponseDto>.Ok(result, "Credential saved."));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponseDto<CredentialListItemResponseDto>>> Update(int id, UpdateCredentialRequestDto dto)
        {
            var result = await _credentialService.UpdateAsync(CurrentUserId, id, dto);
            return Ok(ApiResponseDto<CredentialListItemResponseDto>.Ok(result, "Credential updated."));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
        {
            await _credentialService.DeleteAsync(CurrentUserId, id);
            return Ok(ApiResponseDto<object>.Ok(new { }, "Credential deleted."));
        }
    }
}
