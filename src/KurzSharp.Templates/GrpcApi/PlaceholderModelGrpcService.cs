using KurzSharp.Templates.Database;
using KurzSharp.Templates.Models;
using Microsoft.Extensions.Logging;

#if NET7_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using KurzSharp.GrpcApi;
#endif

namespace KurzSharp.Templates.GrpcApi;

public class PlaceholderModelGrpcService : PlaceholderModelProtoService.PlaceholderModelProtoServiceBase
{
#if NET7_0_OR_GREATER
    private readonly ILogger<PlaceholderModelGrpcService> _logger;
    private readonly KurzSharpDbContext _context;
    private readonly PlaceholderModel _model;

    public PlaceholderModelGrpcService(ILogger<PlaceholderModelGrpcService> logger, KurzSharpDbContext context,
        PlaceholderModel model)
    {
        _logger = logger;
        _context = context;
        _model = model;
    }

    public override async Task<PlaceholderModelDtosResponse> GetProducts(Empty request, ServerCallContext context)
    {
        var allPlaceholderModels = await _context.PlaceholderModels.ToListAsync(context.CancellationToken);

        var dtos = _model.OnBeforeAllRead(allPlaceholderModels);

        var result = new PlaceholderModelDtosResponse();

        result.PlaceholderModelDtos.AddRange(
            dtos.Select(placeholderModel =>
            {
                var dto = _model.OnBeforeRead(placeholderModel);

                return dto.FromModel();
            })
        );

        return result;
    }

    public override async Task<PlaceholderModelDtosResponse> AddProduct(KurzSharp.GrpcApi.PlaceholderModelDto request,
        ServerCallContext context)
    {
        var dto = _model.OnBeforeCreate(request.ToModel());

        var r = await _context.PlaceholderModels.AddAsync(dto, context.CancellationToken);
        await _context.SaveChangesAsync(context.CancellationToken);

        var result = new PlaceholderModelDtosResponse();

        result.PlaceholderModelDtos.Add(r.Entity.FromModel());

        return result;
    }

    public override async Task<PlaceholderModelDtosResponse> DeleteProduct(
        KurzSharp.GrpcApi.PlaceholderModelDto request, ServerCallContext context)
    {
        try
        {
            var dto = _model.OnBeforeDelete(request.ToModel());

            _context.Remove(dto);
            await _context.SaveChangesAsync(context.CancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting {@Entity}, {Message}", request, e.Message);

            throw new RpcException(new Status(StatusCode.Internal, $"Error while trying to delete {request}"),
                $"Error while trying to delete {request}");
        }


        var result = new PlaceholderModelDtosResponse();

        result.PlaceholderModelDtos.Add(request);

        return result;
    }

    public override async Task<PlaceholderModelDtosResponse> UpdateProduct(
        KurzSharp.GrpcApi.PlaceholderModelDto request, ServerCallContext context)
    {
        var dto = _model.OnBeforeUpdate(request.ToModel());

        _context.PlaceholderModels.Update(dto);
        await _context.SaveChangesAsync(context.CancellationToken);

        var result = new PlaceholderModelDtosResponse();

        result.PlaceholderModelDtos.Add(request);

        return result;
    }
#endif
}
