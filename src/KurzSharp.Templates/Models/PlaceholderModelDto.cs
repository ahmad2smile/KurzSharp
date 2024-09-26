using System.Runtime.Serialization;
#if NET8_0_OR_GREATER
using ProtoBuf;
#endif

namespace KurzSharp.Templates.Models;

#if NET8_0_OR_GREATER
[DataContract]
#endif
public class PlaceholderModelDto
{
    public Guid PlaceholderId { get; set; }

#if NET8_0_OR_GREATER
    public PlaceholderModel ToModel()
    {
        var model = new PlaceholderModel();

        model.Id = PlaceholderId;

        return model;
    }
#endif
}
