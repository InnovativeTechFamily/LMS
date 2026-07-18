namespace LMS.Application.Common.Interfaces.Services;

/// <summary>Cloudinary image storage abstraction.</summary>
public interface IMediaStorage
{
    Task<UploadedMedia> UploadAsync(string fileOrDataUri, string folder, int? width = null, CancellationToken ct = default);
    Task DeleteAsync(string publicId, CancellationToken ct = default);
}

public record UploadedMedia(string PublicId, string Url);
