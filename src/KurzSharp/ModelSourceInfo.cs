using Microsoft.CodeAnalysis;

namespace KurzSharp;

public readonly struct ModelSourceInfo(string typeName, string typeNamespace, INamedTypeSymbol? namedTypeSymbol)
{
    public string TypeName { get; } = typeName;
    public string TypeNamespace { get; } = typeNamespace;
    public INamedTypeSymbol? NamedTypeSymbol { get; } = namedTypeSymbol;
}