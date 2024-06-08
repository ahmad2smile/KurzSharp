using KurzSharp.Templates.Models;
using KurzSharp.Templates.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
#if NET7_0_OR_GREATER
using Grpc.Core;
using Microsoft.AspNetCore.Http;
#endif

namespace KurzSharp.Templates.GrpcApi;

#if NET7_0_OR_GREATER
[Tags("PlaceholderModelGrpc")]
#endif
[ApiController]
[Route($"{nameof(PlaceholderModel)}Grpc")]
public class PlaceholderModelGrpcService : ControllerBase, IPlaceholderModelGrpcService
{
#if NET7_0_OR_GREATER
    private readonly ILogger<PlaceholderModelGrpcService> _logger;
    private readonly IPlaceholderModelService _placeholderModelService;

    public PlaceholderModelGrpcService(ILogger<PlaceholderModelGrpcService> logger, IPlaceholderModelService placeholderModelService)
    {
        _logger = logger;
        _placeholderModelService = placeholderModelService;
    }

    [HttpGet]
    public async Task<IEnumerable<PlaceholderModelDto>> GetPlaceholderModels(CancellationToken cancellationToken)
    {
        var result = await _placeholderModelService.GetPlaceholderModels(cancellationToken);

        return result;
    }

    [HttpPost]
    public async Task<PlaceholderModelDto> AddPlaceholderModel(PlaceholderModelDto request, CancellationToken cancellationToken)
    {
        return await _placeholderModelService.AddPlaceholderModel(request, cancellationToken);
    }

    [HttpDelete]
    public async Task<PlaceholderModelDto> DeletePlaceholderModel(PlaceholderModelDto request, CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.DeletePlaceholderModel(request, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting {@Entity}, {Message}", request, e.Message);
    
            throw new RpcException(new Status(Grpc.Core.StatusCode.Internal, $"Error while trying to delete {request}, {e.Message}"),
                $"Error while trying to delete {request}, {e.Message}");
        }
    }
    
    [HttpPut]
    public async Task<PlaceholderModelDto> UpdatePlaceholderModel(PlaceholderModelDto request, CancellationToken cancellationToken)
    {
        return await _placeholderModelService.UpdatePlaceholderModel(request, cancellationToken);
    }
#endif
}
