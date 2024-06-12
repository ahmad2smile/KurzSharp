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
    Task<IEnumerable<PlaceholderModelDto>> GetPlaceholderModels(CancellationToken cancellationToken);

    [OperationContract]
    Task<PlaceholderModelDto> AddPlaceholderModel(PlaceholderModelDto request, CancellationToken cancellationToken);

    [OperationContract]
    Task<PlaceholderModelDto> DeletePlaceholderModel(PlaceholderModelDto request, CancellationToken cancellationToken);

    [OperationContract]
    Task<PlaceholderModelDto> UpdatePlaceholderModel(PlaceholderModelDto request, CancellationToken cancellationToken);
#endif
}