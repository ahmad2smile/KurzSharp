#nullable enable
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

    [HttpGet("{id}")]
    public async Task<PlaceholderModelDto?> GetPlaceholderModel(IdRequest request, CancellationToken cancellationToken) =>
        await _placeholderModelService.GetPlaceholderModel(request.Id, cancellationToken);

    [HttpGet($"/{nameof(PlaceholderModel)}Grpc/bulk")]
    public async Task<IEnumerable<PlaceholderModelDto>> GetPlaceholderModels(CancellationToken cancellationToken) =>
        await _placeholderModelService.GetPlaceholderModels(cancellationToken);

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
        try
        {
            return await _placeholderModelService.AddPlaceholderModel(request, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while adding {@Entity}, {Message}", request, e.Message);

            throw new RpcException(new Status(Grpc.Core.StatusCode.Internal, $"Error while trying to add {request}"),
                $"Error while trying to add {request}");
        }
    }

    [HttpPost($"/{nameof(PlaceholderModel)}Grpc/bulk")]
    public async Task<IEnumerable<PlaceholderModelDto>> AddPlaceholderModels(IEnumerable<PlaceholderModelDto> requests,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.AddPlaceholderModels(requests, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while adding PlaceholderModels, {Message}", e.Message);

            throw new RpcException(
                new Status(Grpc.Core.StatusCode.Internal,
                    $"Error while trying to add PlaceholderModels, {e.Message}"),
                $"Error while trying to to add PlaceholderModels, {e.Message}");
        }
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

    [HttpDelete($"/{nameof(PlaceholderModel)}Grpc/bulk")]
    public async Task<IEnumerable<PlaceholderModelDto>> DeletePlaceholderModels(
        IEnumerable<PlaceholderModelDto> requests, CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.DeletePlaceholderModels(requests, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting PlaceholderModels {Message}", e.Message);

            throw new RpcException(
                new Status(Grpc.Core.StatusCode.Internal,
                    $"Error while trying to delete PlaceholderModels {e.Message}"),
                $"Error while trying to delete PlaceholderModels {e.Message}");
        }
    }

    [HttpPut]
    public async Task<PlaceholderModelDto> UpdatePlaceholderModel(PlaceholderModelDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.UpdatePlaceholderModel(request, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while update {@Entity}, {Message}", request, e.Message);

            throw new RpcException(
                new Status(Grpc.Core.StatusCode.Internal,
                    $"Error while trying to update PlaceholderModels {e.Message}"),
                $"Error while trying to update PlaceholderModels {e.Message}");
        }
    }

    [HttpPut($"/{nameof(PlaceholderModel)}Grpc/bulk")]
    public async Task<IEnumerable<PlaceholderModelDto>> UpdatePlaceholderModels(
        IEnumerable<PlaceholderModelDto> requests, CancellationToken cancellationToken)
    {
        try
        {
            return await _placeholderModelService.UpdatePlaceholderModels(requests, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while update PlaceholderModels {Message}", e.Message);

            throw new RpcException(
                new Status(Grpc.Core.StatusCode.Internal,
                    $"Error while trying to update PlaceholderModels {e.Message}"),
                $"Error while trying to update PlaceholderModels {e.Message}");
        }
    }
}
#endif

public partial class PlaceholderModelGrpcService
{
}
