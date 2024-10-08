#nullable enable
using KurzSharp.Templates.Models;
#if NET8_0_OR_GREATER
using System.ServiceModel;
#endif

namespace KurzSharp.Templates.GrpcApi;

// NOTE: Placeholder for Code Generated by Grpc Tools
#if NET8_0_OR_GREATER
[ServiceContract]
#endif
public interface IPlaceholderModelGrpcService
{
#if NET8_0_OR_GREATER
    [OperationContract]
    Task<PlaceholderModelDto?> GetPlaceholderModel(IdRequest id, CancellationToken cancellationToken);

    [OperationContract]
    Task<IEnumerable<PlaceholderModelDto>> GetPlaceholderModels(CancellationToken cancellationToken);

    [OperationContract]
    Task<PlaceholderModelDto> AddPlaceholderModel(PlaceholderModelDto request, CancellationToken cancellationToken);

    [OperationContract]
    Task<IEnumerable<PlaceholderModelDto>> AddPlaceholderModels(IEnumerable<PlaceholderModelDto> requests, CancellationToken cancellationToken);

    [OperationContract]
    Task<PlaceholderModelDto> DeletePlaceholderModel(PlaceholderModelDto request, CancellationToken cancellationToken);

    [OperationContract]
    Task<IEnumerable<PlaceholderModelDto>> DeletePlaceholderModels(IEnumerable<PlaceholderModelDto> requests, CancellationToken cancellationToken);

    [OperationContract]
    Task<PlaceholderModelDto> UpdatePlaceholderModel(PlaceholderModelDto request, CancellationToken cancellationToken);

    [OperationContract]
    Task<IEnumerable<PlaceholderModelDto>> UpdatePlaceholderModels(IEnumerable<PlaceholderModelDto> requests, CancellationToken cancellationToken);
#endif
}
