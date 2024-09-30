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

    private static List<ApiKind> ParseApiKind(IReadOnlyCollection<AttributeSyntax> attributeSyntaxes) =>
        attributeSyntaxes.Select(attributeSyntax =>
        {
            var attributeName = attributeSyntax.Name + "Attribute";

            return attributeName switch
            {
                nameof(RestApiAttribute) => ApiKind.Rest,
                nameof(GrpcApiAttribute) => ApiKind.Grpc,
                nameof(GraphQlApiAttribute) => ApiKind.GraphQl,
                nameof(AdminDashboardAttribute) => ApiKind.AdminDashboard,
                _ => throw new ArgumentException($"Unknown ApiKind: {attributeSyntax.Name.ToString()}")
            };
        }).ToList();
}
