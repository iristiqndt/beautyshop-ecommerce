using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _webRootPath;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileStorageService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        // Set web root path to wwwroot folder
        _webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder)
    {
        try
        {
            // Create unique filename
            var extension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            
            // Create folder path
            var uploadFolder = Path.Combine(_webRootPath, folder);
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // Save file
            var filePath = Path.Combine(uploadFolder, uniqueFileName);
            using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamOutput);
            }

            // Return URL
            return GetFileUrl($"{folder}/{uniqueFileName}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error uploading file: {ex.Message}");
        }
    }

    public async Task<bool> DeleteFileAsync(string fileUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(fileUrl))
                return false;

            // Extract relative path from URL
            var uri = new Uri(fileUrl);
            var relativePath = uri.AbsolutePath.TrimStart('/');
            
            var filePath = Path.Combine(_webRootPath, relativePath);
            
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public string GetFileUrl(string fileName)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null)
            return fileName;

        var baseUrl = $"{request.Scheme}://{request.Host}";
        return $"{baseUrl}/{fileName.Replace("\\", "/")}";
    }
}
