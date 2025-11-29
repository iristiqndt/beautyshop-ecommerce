using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommerce.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Services;

public class CloudinaryStorageService : IFileStorageService
{
    private readonly Cloudinary _cloudinary;
    private readonly IConfiguration _configuration;
    private readonly string _uploadsPath;

    public CloudinaryStorageService(IConfiguration configuration)
    {
        _configuration = configuration;
        
        // If Cloudinary is not properly configured, use local file storage
        if (IsCloudinaryConfigured())
        {
            var account = new Account(
                _configuration["Cloudinary:CloudName"],
                _configuration["Cloudinary:ApiKey"],
                _configuration["Cloudinary:ApiSecret"]);
            
            _cloudinary = new Cloudinary(account);
        }
        else
        {
            _cloudinary = null;
        }

        // Setup local uploads directory
        _uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(_uploadsPath))
        {
            Directory.CreateDirectory(_uploadsPath);
        }
    }

    private bool IsCloudinaryConfigured()
    {
        var cloudName = _configuration["Cloudinary:CloudName"];
        var apiKey = _configuration["Cloudinary:ApiKey"];
        var apiSecret = _configuration["Cloudinary:ApiSecret"];
        
        return !string.IsNullOrEmpty(cloudName) && 
               !cloudName.Contains("your-") &&
               !string.IsNullOrEmpty(apiKey) && 
               !apiKey.Contains("your-") &&
               !string.IsNullOrEmpty(apiSecret) && 
               !apiSecret.Contains("your-");
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder)
    {
        // If Cloudinary is configured, use it
        if (_cloudinary != null)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = folder,
                Transformation = new Transformation().Width(800).Height(800).Crop("limit")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            
            if (uploadResult.Error != null)
            {
                throw new Exception($"File upload failed: {uploadResult.Error.Message}");
            }

            return uploadResult.SecureUrl.ToString();
        }
        
        // Otherwise, save locally
        return await SaveFileLocally(fileStream, fileName, folder);
    }

    private async Task<string> SaveFileLocally(Stream fileStream, string fileName, string folder)
    {
        try
        {
            // Create unique filename
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var folderPath = Path.Combine(_uploadsPath, folder);
            
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, uniqueFileName);

            using (var fileToSave = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileToSave);
            }

            // Return relative URL for the file
            return $"/uploads/{folder}/{uniqueFileName}";
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to save file locally: {ex.Message}");
        }
    }

    public async Task<bool> DeleteFileAsync(string fileUrl)
    {
        try
        {
            // If using Cloudinary
            if (_cloudinary != null)
            {
                var publicId = GetPublicIdFromUrl(fileUrl);
                var deletionParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deletionParams);
                return result.Result == "ok";
            }
            
            // If using local storage
            if (fileUrl.StartsWith("/uploads/"))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
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
        if (_cloudinary != null)
        {
            return _cloudinary.Api.UrlImgUp.BuildUrl(fileName);
        }
        
        return $"/uploads/{fileName}";
    }

    private string GetPublicIdFromUrl(string url)
    {
        var uri = new Uri(url);
        var segments = uri.AbsolutePath.Split('/');
        var publicIdWithExtension = string.Join("/", segments.Skip(segments.Length - 2));
        return publicIdWithExtension.Substring(0, publicIdWithExtension.LastIndexOf('.'));
    }
}
