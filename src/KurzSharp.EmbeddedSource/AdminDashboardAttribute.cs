namespace KurzSharp;

/// <summary>
/// Generates A Admin Dashboard UI for given Model
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class AdminDashboardAttribute : Attribute
{
    /// <summary>
    /// Route of page based on Blazor @page "/{Route}" attribute template
    /// </summary>
    public string? Route { get; set; }
}
