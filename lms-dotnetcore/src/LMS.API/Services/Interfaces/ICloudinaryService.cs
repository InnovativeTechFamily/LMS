namespace LMS.API.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<(string publicId, string url)> UploadImageAsync(string base64Image, string folder = "avatars");
        Task<bool> DeleteImageAsync(string publicId);
    }
}
