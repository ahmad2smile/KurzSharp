#nullable enable
using KurzSharp.Templates.Models;

namespace KurzSharp.Templates.Services;

public interface IPlaceholderModelService
{
#if NET8_0_OR_GREATER
    Task<PlaceholderModelDto?> GetPlaceholderModel(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<PlaceholderModelDto>> GetPlaceholderModels(CancellationToken cancellationToken);

    Task<PlaceholderModelDto> AddPlaceholderModel(PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken);

    Task<IEnumerable<PlaceholderModelDto>> AddPlaceholderModels(IEnumerable<PlaceholderModelDto> placeholderModelDtos,
        CancellationToken cancellationToken);

    Task<PlaceholderModelDto> DeletePlaceholderModel(PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken);

    Task<IEnumerable<PlaceholderModelDto>> DeletePlaceholderModels(
        IEnumerable<PlaceholderModelDto> placeholderModelDtos, CancellationToken cancellationToken);

    Task<PlaceholderModelDto> UpdatePlaceholderModel(PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken);

    Task<IEnumerable<PlaceholderModelDto>> UpdatePlaceholderModels(
        IEnumerable<PlaceholderModelDto> placeholderModelDtos, CancellationToken cancellationToken);
#endif
}
