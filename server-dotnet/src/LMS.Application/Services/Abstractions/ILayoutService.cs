using LMS.Application.DTOs.Layouts;
using LMS.Domain.Entities;

namespace LMS.Application.Services.Abstractions;

public interface ILayoutService
{
    Task CreateAsync(LayoutRequest request, CancellationToken ct = default);
    Task EditAsync(LayoutRequest request, CancellationToken ct = default);
    Task<Layout?> GetByTypeAsync(string type, CancellationToken ct = default);
}
