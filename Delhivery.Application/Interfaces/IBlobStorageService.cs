using Delhivery.Domain.Entities;

namespace Delhivery.Application.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<bool> DeleteFileAsync(string fileUrl);
}
