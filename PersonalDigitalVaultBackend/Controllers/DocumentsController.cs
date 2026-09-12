using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalDigitalVaultBackend.DTOs.RequestDtos.Documents;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Common;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Documents;
using PersonalDigitalVaultBackend.Services.Interface;
using System.Security.Claims;

namespace PersonalDigitalVaultBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        public DocumentsController(IDocumentService documentService) => _documentService = documentService;

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<List<DocumentResponesDto>>>> GetAll(
            [FromQuery] int? folderId, [FromQuery] string? search)
        {
            var result = await _documentService.GetAllAsync(CurrentUserId, folderId, search);
            return Ok(ApiResponseDto<List<DocumentResponesDto>>.Ok(result));
        }

        [HttpPost("upload")]
        [RequestSizeLimit(30_000_000)]
        public async Task<ActionResult<ApiResponseDto<DocumentResponesDto>>> Upload(
            IFormFile file, [FromForm] int? folderId)
        {
            var result = await _documentService.UploadAsync(CurrentUserId, file, folderId);
            return Ok(ApiResponseDto<DocumentResponesDto>.Ok(result, "File uploaded and encrypted."));
        }

        [HttpGet("{id:int}/download")]
        public async Task<IActionResult> Download(int id)
        {
            var (content, fileName, contentType) = await _documentService.DownloadAsync(CurrentUserId, id);
            return File(content, contentType, fileName);
        }

        // BRD FR#7: "User searches, views, downloads, renames, or deletes
        // items" - "views" is a distinct action from "downloads". No
        // fileDownloadName is passed here, so ASP.NET Core doesn't set
        // Content-Disposition: attachment, letting the browser render
        // previewable types (images, PDF) inline in a new tab instead of
        // forcing a save-file dialog.
        [HttpGet("{id:int}/view")]
        public async Task<IActionResult> View(int id)
        {
            var (content, _, contentType) = await _documentService.DownloadAsync(CurrentUserId, id);
            return File(content, contentType);
        }

        [HttpPut("{id:int}/rename")]
        public async Task<ActionResult<ApiResponseDto<DocumentResponesDto>>> Rename(int id, RenameDocumentRequestDto dto)
        {
            var result = await _documentService.RenameAsync(CurrentUserId, id, dto);
            return Ok(ApiResponseDto<DocumentResponesDto>.Ok(result, "Document renamed."));
        }

        [HttpPut("{id:int}/move")]
        public async Task<ActionResult<ApiResponseDto<DocumentResponesDto>>> Move(int id, MoveDocumentRequestDto dto)
        {
            var result = await _documentService.MoveAsync(CurrentUserId, id, dto);
            return Ok(ApiResponseDto<DocumentResponesDto>.Ok(result, "Document moved."));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
        {
            await _documentService.DeleteAsync(CurrentUserId, id);
            return Ok(ApiResponseDto<object>.Ok(new { }, "Document deleted."));
        }
    }
}
