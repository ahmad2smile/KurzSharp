namespace KurzSharp;

/// <summary>
/// Generates A REST API for given Model
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class RestApiAttribute: Attribute
{
    /// <summary>
    /// Route of endpoint based on ASP.NET Core Controller attribute template
    /// </summary>
    public string? Route { get; set; }
}