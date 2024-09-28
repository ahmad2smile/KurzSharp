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

    [Error(typeof(GraphQlExceptions.AddEntityException))]
    [UseMutationConvention(InputArgumentName = "input", InputTypeName = nameof(PlaceholderModelDto),
        PayloadFieldName = "placeholderModel")]
    public async Task<PlaceholderModelDto> AddPlaceholderModel(PlaceholderModelDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.AddPlaceholderModel(input, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while adding {@Entity}, {Message}", input, e.Message);

            throw new GraphQlExceptions.AddEntityException(e.Message);
        }
    }

    [Error(typeof(GraphQlExceptions.AddEntitiesException))]
    [UseMutationConvention(InputArgumentName = "input", InputTypeName = nameof(IEnumerable<PlaceholderModelDto>),
        PayloadFieldName = "placeholderModel")]
    public async Task<IEnumerable<PlaceholderModelDto>> AddPlaceholderModels(IEnumerable<PlaceholderModelDto> input,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.AddPlaceholderModels(input, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while adding PlaceholderModels, {Message}", e.Message);

            throw new GraphQlExceptions.AddEntitiesException(e.Message);
        }
    }

    [Error(typeof(GraphQlExceptions.DeleteEntityException))]
    [UseMutationConvention(InputArgumentName = "input", InputTypeName = nameof(PlaceholderModelDto),
        PayloadFieldName = "placeholderModel")]
    public async Task<PlaceholderModelDto> DeletePlaceholderModel(PlaceholderModelDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.DeletePlaceholderModel(input, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting {@Entity}, {Message}", input, e.Message);

            throw new GraphQlExceptions.DeleteEntityException(e.Message);
        }
    }

    [Error(typeof(GraphQlExceptions.DeleteEntitiesException))]
    [UseMutationConvention(InputArgumentName = "input", InputTypeName = nameof(IEnumerable<PlaceholderModelDto>),
        PayloadFieldName = "placeholderModel")]
    public async Task<IEnumerable<PlaceholderModelDto>> DeletePlaceholderModels(IEnumerable<PlaceholderModelDto> input,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.DeletePlaceholderModels(input, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting PlaceholderModels {Message}", e.Message);

            throw new GraphQlExceptions.DeleteEntitiesException(e.Message);
        }
    }

    [Error(typeof(GraphQlExceptions.UpdateEntityException))]
    [UseMutationConvention(InputArgumentName = "input", InputTypeName = nameof(PlaceholderModelDto),
        PayloadFieldName = "placeholderModel")]
    public async Task<PlaceholderModelDto> UpdatePlaceholderModel(PlaceholderModelDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.UpdatePlaceholderModel(input, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while update {@Entity}, {Message}", input, e.Message);

            throw new GraphQlExceptions.UpdateEntityException(e.Message);
        }
    }

    [Error(typeof(GraphQlExceptions.UpdateEntitiesException))]
    [UseMutationConvention(InputArgumentName = "input", InputTypeName = nameof(IEnumerable<PlaceholderModelDto>),
        PayloadFieldName = "placeholderModel")]
    public async Task<IEnumerable<PlaceholderModelDto>> UpdatePlaceholderModels(IEnumerable<PlaceholderModelDto> input,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.UpdatePlaceholderModels(input, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while update PlaceholderModels {Message}", e.Message);

            throw new GraphQlExceptions.UpdateEntitiesException(e.Message);
        }
    }
#endif
}
