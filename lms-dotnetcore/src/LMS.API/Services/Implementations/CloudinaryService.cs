using LMS.API.Configuration;
using LMS.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace LMS.API.Services.Implementations
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(IOptions<CloudinarySettings> settings, ILogger<CloudinaryService> logger)
        {
            var cloudinarySettings = settings.Value;
            _cloudinary = new Cloudinary(new Account(
                cloudinarySettings.CloudName,
                cloudinarySettings.ApiKey,
                cloudinarySettings.ApiSecret
            ));
            _logger = logger;
        }

        public async Task<(string publicId, string url)> UploadImageAsync(string base64Image, string folder = "avatars")
        {
            _logger.LogInformation("Uploading image to Cloudinary folder: {Folder}", folder);

            try
            {
                // Convert base64 to byte array
                var imageBytes = Convert.FromBase64String(base64Image.Contains(",") ? base64Image.Split(",")[1] : base64Image);
                using (var stream = new MemoryStream(imageBytes))
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription($"upload_{Guid.NewGuid()}", stream),
                        Folder = folder,
                        Width = 150,
                        Height = 150,
                        Crop = "fill"
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                    {
                        _logger.LogError("Cloudinary upload error: {Error}", uploadResult.Error.Message);
                        throw new InvalidOperationException($"Upload failed: {uploadResult.Error.Message}");
                    }

                    _logger.LogInformation("Image uploaded successfully. Public ID: {PublicId}", uploadResult.PublicId);
                    return (uploadResult.PublicId, uploadResult.SecureUrl.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to Cloudinary");
                throw;
            }
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            _logger.LogInformation("Deleting image from Cloudinary. Public ID: {PublicId}", publicId);

            try
            {
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary delete error: {Error}", result.Error.Message);
                    return false;
                }

                _logger.LogInformation("Image deleted successfully. Public ID: {PublicId}", publicId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image from Cloudinary. Public ID: {PublicId}", publicId);
                return false;
            }
        }
    }
}
