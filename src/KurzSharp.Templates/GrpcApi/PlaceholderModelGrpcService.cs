#if NET8_0_OR_GREATER
using ProtoBuf.Grpc.Reflection;
using ProtoBuf.Meta;
using Grpc.Core;
using KurzSharp.Templates.Models;
using KurzSharp.Templates.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
#endif

namespace KurzSharp.Templates.GrpcApi;

#if NET8_0_OR_GREATER
[Tags("PlaceholderModelGrpc")]
[ApiController]
[Route($"{nameof(PlaceholderModel)}Grpc")]
public partial class PlaceholderModelGrpcService : ControllerBase, IPlaceholderModelGrpcService
{
    private readonly ILogger<PlaceholderModelGrpcService> _logger;
    private readonly IPlaceholderModelService _placeholderModelService;

    public PlaceholderModelGrpcService(ILogger<PlaceholderModelGrpcService> logger,
        IPlaceholderModelService placeholderModelService)
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

    [HttpGet("proto")]
    public string GetPlaceholderModelProto(CancellationToken cancellationToken)
    {
        var generator = new SchemaGenerator
        {
            ProtoSyntax = ProtoSyntax.Proto3
        };

        return generator.GetSchema<IPlaceholderModelGrpcService>();
    }

    [HttpPost]
    public async Task<PlaceholderModelDto> AddPlaceholderModel(PlaceholderModelDto request,
        CancellationToken cancellationToken)
    {
        return await _placeholderModelService.AddPlaceholderModel(request, cancellationToken);
    }

    [HttpDelete]
    public async Task<PlaceholderModelDto> DeletePlaceholderModel(PlaceholderModelDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.DeletePlaceholderModel(request, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting {@Entity}, {Message}", request, e.Message);

            throw new RpcException(new Status(Grpc.Core.StatusCode.Internal, $"Error while trying to delete {request}"),
                $"Error while trying to delete {request}");
        }
    }

    [HttpPut]
    public async Task<PlaceholderModelDto> UpdatePlaceholderModel(PlaceholderModelDto request,
        CancellationToken cancellationToken)
    {
        return await _placeholderModelService.UpdatePlaceholderModel(request, cancellationToken);
    }
}
#endif

public partial class PlaceholderModelGrpcService
{
}
