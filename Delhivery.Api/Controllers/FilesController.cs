using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Delhivery.Application.Interfaces;
using Delhivery.Domain.Entities;

namespace Delhivery.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly IRepository<FileMetadata> _fileRepository;

    public FilesController(IBlobStorageService blobStorageService, IRepository<FileMetadata> fileRepository)
    {
        _blobStorageService = blobStorageService;
        _fileRepository = fileRepository;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        using var stream = file.OpenReadStream();
        var url = await _blobStorageService.UploadFileAsync(stream, fileName, file.ContentType);

        var fileMetadata = new FileMetadata
        {
            FileName = fileName,
            BlobUrl = url,
            ContentType = file.ContentType,
            UploadedById = GetUserId()
        };

        await _fileRepository.AddAsync(fileMetadata);

        return Ok(new { fileMetadata.Id, fileMetadata.BlobUrl });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFileMetadata(Guid id)
    {
        var file = await _fileRepository.GetByIdAsync(id);
        if (file == null)
            return NotFound("File not found");

        return Ok(file);
    }
}
