using Microsoft.CodeAnalysis;

namespace KurzSharp.Utils;

public static class DiagnosticDescriptors
{
    private const string Category = "KurzSharp";

    public static readonly DiagnosticDescriptor ModelMustBePartial = new(
        "KS001",
        "KurzSharp model must be partial",
        "Type '{0}' is decorated with a KurzSharp attribute but is not declared 'partial'. Add the 'partial' modifier so KurzSharp can generate the API for it.",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ModelMustHaveProperties = new(
        "KS002",
        "KurzSharp model has no properties",
        "Type '{0}' has no properties; KurzSharp needs at least one property (used as the key) to generate an API.",
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ModelMissingKey = new(
        "KS003",
        "KurzSharp model has no primary key",
        "Type '{0}' has no '[Key]' property, no 'Id' property and no '{0}Id' property. EF Core will fail to determine a primary key at runtime. Add one of these, or configure the key in a partial KurzSharpDbContext.",
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
