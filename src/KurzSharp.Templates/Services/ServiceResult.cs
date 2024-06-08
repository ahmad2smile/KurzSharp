#if NET7_0_OR_GREATER
using System.Runtime.Serialization;
using Grpc.Core;
#endif

namespace KurzSharp.Templates.Services;

#if NET7_0_OR_GREATER
[DataContract]
#endif
public class ServiceResult<TData>
{
#if NET7_0_OR_GREATER
    public ServiceResult(TData data)
    {
        Data = data;
    }

    public ServiceResult(TData data, StatusCode statusCode)
    {
        Data = data;
        StatusCode = statusCode;
    }

    [DataMember(Order = 1)]
    public TData Data { get; init; }

    [DataMember(Order = 2)]
    public StatusCode StatusCode { get; init; } = StatusCode.OK;
#endif
}
