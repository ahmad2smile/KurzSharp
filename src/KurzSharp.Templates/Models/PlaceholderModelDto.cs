using System.Runtime.Serialization;
#if NET7_0_OR_GREATER
using ProtoBuf;
#endif

namespace KurzSharp.Templates.Models;

#if NET7_0_OR_GREATER
[DataContract]
#endif
public class PlaceholderModelDto
{
    [DataMember(Order = 1)]
    public Guid PlaceholderId { get; set; }

#if NET7_0_OR_GREATER
    public PlaceholderModel CopyToModel(PlaceholderModel model)
    {
        model.Id = PlaceholderId;

        return model;
    }
#endif
}
