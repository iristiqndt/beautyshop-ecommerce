namespace ECommerce.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder);
    Task<bool> DeleteFileAsync(string fileUrl);
    string GetFileUrl(string fileName);
}
