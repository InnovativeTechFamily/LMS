using LMS.Application.Common.Exceptions;
using LMS.Application.Common.Interfaces.Persistence;
using LMS.Application.Common.Interfaces.Services;
using LMS.Application.DTOs.Layouts;
using LMS.Application.Services.Abstractions;
using LMS.Domain.Entities;

namespace LMS.Application.Services;

public class LayoutService : ILayoutService
{
    private readonly ILayoutRepository _layouts;
    private readonly IMediaStorage _media;

    public LayoutService(ILayoutRepository layouts, IMediaStorage media)
    {
        _layouts = layouts;
        _media = media;
    }

    public async Task CreateAsync(LayoutRequest request, CancellationToken ct = default)
    {
        if (await _layouts.GetByTypeAsync(request.Type, ct) is not null)
            throw new BadRequestException($"{request.Type} already exist");

        var layout = new Layout { Type = request.Type };

        switch (request.Type)
        {
            case LayoutTypes.Banner:
                var uploaded = await _media.UploadAsync(request.Image ?? string.Empty, folder: "layout", width: null, ct);
                layout.Banner = new Banner
                {
                    Image = new MediaFile { PublicId = uploaded.PublicId, Url = uploaded.Url },
                    Title = request.Title ?? string.Empty,
                    SubTitle = request.SubTitle ?? string.Empty,
                };
                break;

            case LayoutTypes.Faq:
                layout.Faq = (request.Faq ?? new()).Select(f => new FaqItem { Question = f.Question, Answer = f.Answer }).ToList();
                break;

            case LayoutTypes.Categories:
                layout.Categories = (request.Categories ?? new()).Select(c => new Category { Title = c.Title }).ToList();
                break;
        }

        await _layouts.AddAsync(layout, ct);
    }

    public async Task EditAsync(LayoutRequest request, CancellationToken ct = default)
    {
        var layout = await _layouts.GetByTypeAsync(request.Type, ct)
            ?? throw new NotFoundException($"{request.Type} layout not found");

        switch (request.Type)
        {
            case LayoutTypes.Banner:
                var image = request.Image ?? string.Empty;
                MediaFile media;
                if (image.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    media = layout.Banner?.Image ?? new MediaFile { Url = image };
                }
                else
                {
                    var uploaded = await _media.UploadAsync(image, folder: "layout", width: null, ct);
                    media = new MediaFile { PublicId = uploaded.PublicId, Url = uploaded.Url };
                }

                layout.Banner = new Banner
                {
                    Image = media,
                    Title = request.Title ?? string.Empty,
                    SubTitle = request.SubTitle ?? string.Empty,
                };
                break;

            case LayoutTypes.Faq:
                layout.Faq = (request.Faq ?? new()).Select(f => new FaqItem { Question = f.Question, Answer = f.Answer }).ToList();
                break;

            case LayoutTypes.Categories:
                layout.Categories = (request.Categories ?? new()).Select(c => new Category { Title = c.Title }).ToList();
                break;
        }

        await _layouts.UpdateAsync(layout, ct);
    }

    public Task<Layout?> GetByTypeAsync(string type, CancellationToken ct = default)
        => _layouts.GetByTypeAsync(type, ct);
}
