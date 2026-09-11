using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalDigitalVaultBackend.DTOs.RequestDtos.Folders;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Common;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Folders;
using PersonalDigitalVaultBackend.Services.Interface;
using System.Security.Claims;

namespace PersonalDigitalVaultBackend.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FoldersController : ControllerBase
    {
        private readonly IFolderService _folderService;
        public FoldersController(IFolderService folderService) => _folderService = folderService;

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<List<FolderResponseDto>>>> GetAll()
        {
            var result = await _folderService.GetAllAsync(CurrentUserId);
            return Ok(ApiResponseDto<List<FolderResponseDto>>.Ok(result));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<FolderResponseDto>>> Create(CreateFolderRequestDto dto)
        {
            var result = await _folderService.CreateAsync(CurrentUserId, dto);
            return Ok(ApiResponseDto<FolderResponseDto>.Ok(result, "Folder created."));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponseDto<FolderResponseDto>>> Rename(int id, RenameFolderRequestDto dto)
        {
            var result = await _folderService.RenameAsync(CurrentUserId, id, dto);
            return Ok(ApiResponseDto<FolderResponseDto>.Ok(result, "Folder renamed."));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
        {
            await _folderService.DeleteAsync(CurrentUserId, id);
            return Ok(ApiResponseDto<object>.Ok(new { }, "Folder deleted."));
        }
    }

}
