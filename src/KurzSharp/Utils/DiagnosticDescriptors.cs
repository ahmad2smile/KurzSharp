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
}
