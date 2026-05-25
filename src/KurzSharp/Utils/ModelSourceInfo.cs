using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KurzSharp.Utils;

public class ModelSourceInfo(
    string typeName,
    string typeNamespace,
    INamedTypeSymbol namedTypeSymbol,
    IReadOnlyCollection<AttributeSyntax> attributeSyntaxes,
    IReadOnlyCollection<ModelProperty> properties)
{
    public string TypeName { get; } = typeName;
    public string TypeNamespace { get; } = typeNamespace;
    public INamedTypeSymbol NamedTypeSymbol { get; } = namedTypeSymbol;
    public IReadOnlyCollection<ApiKind> ApiKinds { get; } = ParseApiKind(attributeSyntaxes);
    public IReadOnlyCollection<ModelProperty> Properties { get; } = properties;
    public ModelProperty? KeyProperty { get; } = ResolveKey(typeName, properties);

    private static List<ApiKind> ParseApiKind(IReadOnlyCollection<AttributeSyntax> attributeSyntaxes) =>
        attributeSyntaxes
            .Select(attributeSyntax =>
            {
                var name = attributeSyntax.Name.ToString();

                // Attributes may be written fully-qualified (KurzSharp.RestApi); keep only the last segment.
                var lastDot = name.LastIndexOf('.');
                if (lastDot >= 0)
                {
                    name = name.Substring(lastDot + 1);
                }

                // Attributes may be written with or without the optional `Attribute` postfix.
                if (!name.EndsWith("Attribute"))
                {
                    name += "Attribute";
                }

                return name switch
                {
                    nameof(RestApiAttribute) => (ApiKind?)ApiKind.Rest,
                    nameof(GrpcApiAttribute) => ApiKind.Grpc,
                    nameof(GraphQlApiAttribute) => ApiKind.GraphQl,
                    nameof(AdminDashboardAttribute) => ApiKind.AdminDashboard,
                    // NOTE: Non-KurzSharp attributes on the model are ignored rather than treated as an error.
                    _ => null
                };
            })
            .Where(kind => kind.HasValue)
            .Select(kind => kind!.Value)
            .Distinct()
            .ToList();

    // EF Core's primary-key convention: a [Key] attribute, a property named 'Id', or a property
    // named '{TypeName}Id' (all case-insensitive). Returns null when none match so callers can tell
    // a real key apart from the positional fallback. This is the single source of truth shared with
    // the KS003 "missing key" diagnostic.
    public static ModelProperty? FindConventionalKey(string typeName, IReadOnlyCollection<ModelProperty> properties) =>
        properties.FirstOrDefault(p => p.Attributes.Any(a => a.AttributeClass?.Name == "KeyAttribute")) ??
        properties.FirstOrDefault(p => string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase)) ??
        properties.FirstOrDefault(p => string.Equals(p.Name, $"{typeName}Id", StringComparison.OrdinalIgnoreCase));

    // NOTE: The generated CRUD code looks up entities by their key. Resolve it once so every template
    // agrees on the same key name/type, falling back to the first property when no conventional key
    // exists (KS003 warns in that case).
    private static ModelProperty? ResolveKey(string typeName, IReadOnlyCollection<ModelProperty> properties) =>
        FindConventionalKey(typeName, properties) ?? properties.FirstOrDefault();
}
