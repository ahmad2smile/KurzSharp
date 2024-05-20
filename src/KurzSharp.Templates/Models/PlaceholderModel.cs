#if NET7_0_OR_GREATER
using System.Text.Json.Serialization;
using KurzSharp;

namespace KurzSharp.Templates.Models;

public partial class PlaceholderModel : LifecycleHooks<PlaceholderModelDto>
{
    [JsonConstructor]
    private PlaceholderModel()
    {
    }

    public Guid Id { get; set; }
}
#endif

public partial class PlaceholderModel
{
}