using KurzSharp.Templates.Services;
using KurzSharp.Templates.Models;
#if NET8_0_OR_GREATER
using Microsoft.Extensions.Logging;
using HotChocolate.Data;
using HotChocolate.Types;
#endif

namespace KurzSharp.Templates.GraphQlApi;

#if NET8_0_OR_GREATER
[ExtendObjectType(typeof(Mutation))]
#endif
public class PlaceholderModelMutation
{
#if NET8_0_OR_GREATER
    private readonly ILogger<PlaceholderModelMutation> _logger;
    private readonly IPlaceholderModelService _placeholderModelService;

    public PlaceholderModelMutation(ILogger<PlaceholderModelMutation> logger,
        IPlaceholderModelService placeholderModelService)
    {
        _logger = logger;
        _placeholderModelService = placeholderModelService;
    }

    public async Task<PlaceholderModelDto> AddPlaceholderModel(PlaceholderModelDto input,
        CancellationToken cancellationToken)
    {
        return await _placeholderModelService.AddPlaceholderModel(input, cancellationToken);
    }

    public async Task<PlaceholderModelDto> DeletePlaceholderModel(PlaceholderModelDto input,
        CancellationToken cancellationToken)
    {
        return await _placeholderModelService.DeletePlaceholderModel(input, cancellationToken);
    }

    public async Task<PlaceholderModelDto> UpdatePlaceholderModel(PlaceholderModelDto input,
        CancellationToken cancellationToken)
    {
        return await _placeholderModelService.UpdatePlaceholderModel(input, cancellationToken);
    }
#endif
}
