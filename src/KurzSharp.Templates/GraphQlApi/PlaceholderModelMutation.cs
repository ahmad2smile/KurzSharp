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
    [UseMutationConvention(PayloadFieldName = "placeholderModel")]
    public async Task<PlaceholderModelDto> AddPlaceholderModel(PlaceholderModelDto placeholderModel,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.AddPlaceholderModel(placeholderModel, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while adding {@Entity}, {Message}", placeholderModel, e.Message);

            throw new GraphQlExceptions.AddEntityException(e.Message);
        }
    }

    [Error(typeof(GraphQlExceptions.AddEntitiesException))]
    [UseMutationConvention(PayloadFieldName = "placeholderModels")]
    public async Task<IEnumerable<PlaceholderModelDto>> AddPlaceholderModels(
        IEnumerable<PlaceholderModelDto> placeholderModels, CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.AddPlaceholderModels(placeholderModels, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while adding PlaceholderModels, {Message}", e.Message);

            throw new GraphQlExceptions.AddEntitiesException(e.Message);
        }
    }

    [Error(typeof(GraphQlExceptions.DeleteEntityException))]
    [UseMutationConvention(PayloadFieldName = "placeholderModel")]
    public async Task<PlaceholderModelDto> DeletePlaceholderModel(PlaceholderModelDto placeholderModel,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.DeletePlaceholderModel(placeholderModel, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting {@Entity}, {Message}", placeholderModel, e.Message);

            throw new GraphQlExceptions.DeleteEntityException(e.Message);
        }
    }

    [Error(typeof(GraphQlExceptions.DeleteEntitiesException))]
    [UseMutationConvention(PayloadFieldName = "placeholderModels")]
    public async Task<IEnumerable<PlaceholderModelDto>> DeletePlaceholderModels(
        IEnumerable<PlaceholderModelDto> placeholderModels, CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.DeletePlaceholderModels(placeholderModels, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting PlaceholderModels {Message}", e.Message);

            throw new GraphQlExceptions.DeleteEntitiesException(e.Message);
        }
    }

    [Error(typeof(GraphQlExceptions.UpdateEntityException))]
    [UseMutationConvention(PayloadFieldName = "placeholderModel")]
    public async Task<PlaceholderModelDto> UpdatePlaceholderModel(PlaceholderModelDto placeholderModel,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.UpdatePlaceholderModel(placeholderModel, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while update {@Entity}, {Message}", placeholderModel, e.Message);

            throw new GraphQlExceptions.UpdateEntityException(e.Message);
        }
    }

    [Error(typeof(GraphQlExceptions.UpdateEntitiesException))]
    [UseMutationConvention(PayloadFieldName = "placeholderModels")]
    public async Task<IEnumerable<PlaceholderModelDto>> UpdatePlaceholderModels(
        IEnumerable<PlaceholderModelDto> placeholderModels, CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.UpdatePlaceholderModels(placeholderModels, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while update PlaceholderModels {Message}", e.Message);

            throw new GraphQlExceptions.UpdateEntitiesException(e.Message);
        }
    }
#endif
}
