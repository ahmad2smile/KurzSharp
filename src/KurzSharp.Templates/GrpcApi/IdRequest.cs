#if NET8_0_OR_GREATER
using ProtoBuf;
#endif

namespace KurzSharp.Templates.GrpcApi;

#if NET8_0_OR_GREATER
[ProtoContract]
#endif
public class IdRequest
{
#if NET8_0_OR_GREATER
    [ProtoMember(1)]
#endif
    public Guid Id { get; set; }
}
