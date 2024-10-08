#if NET8_0_OR_GREATER
using System.Text.Json.Serialization;
#endif
// NOTE: Is important in final generated code to get `LifeCycleHooks`
// ReSharper disable once RedundantUsingDirective
using KurzSharp;

namespace KurzSharp.Templates.Models;

#if NET8_0_OR_GREATER
public partial class PlaceholderModel : LifecycleHooks<PlaceholderModelDto>
{
    [JsonConstructor]
    public PlaceholderModel()
    {
    }

    public Guid Id { get; set; }
}
#endif

public partial class PlaceholderModel
{
}
