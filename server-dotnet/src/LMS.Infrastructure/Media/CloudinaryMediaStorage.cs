using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LMS.Application.Common.Interfaces.Services;
using LMS.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace LMS.Infrastructure.Media;

/// <summary>Cloudinary implementation of <see cref="IMediaStorage"/>. Accepts base64 data URIs or urls.</summary>
public class CloudinaryMediaStorage : IMediaStorage
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryMediaStorage(IOptions<CloudinarySettings> options)
    {
        var s = options.Value;
        _cloudinary = new Cloudinary(new Account(s.CloudName, s.ApiKey, s.ApiSecret));
    }

    public async Task<UploadedMedia> UploadAsync(string fileOrDataUri, string folder, int? width = null, CancellationToken ct = default)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileOrDataUri),
            Folder = folder,
        };

        if (width is not null)
            uploadParams.Transformation = new Transformation().Width(width.Value).Crop("scale");

        var result = await _cloudinary.UploadAsync(uploadParams, ct);
        if (result.Error is not null)
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");

        return new UploadedMedia(result.PublicId, result.SecureUrl.ToString());
    }

    public async Task DeleteAsync(string publicId, CancellationToken ct = default)
        => await _cloudinary.DestroyAsync(new DeletionParams(publicId));
}
