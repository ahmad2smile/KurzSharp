using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KurzSharp.Utils;

public class ModelSourceInfo(
    string typeName,
    string typeNamespace,
    INamedTypeSymbol namedTypeSymbol,
    IList<AttributeSyntax> attributeSyntaxes) : IEquatable<ModelSourceInfo>
{
    public string TypeName { get; } = typeName;
    public string TypeNamespace { get; } = typeNamespace;
    public INamedTypeSymbol NamedTypeSymbol { get; } = namedTypeSymbol;
    public IList<ApiKind> ApiKinds { get; } = ParseApiKind(attributeSyntaxes);

    private static IList<ApiKind> ParseApiKind(IList<AttributeSyntax> attributeSyntaxes) =>
        attributeSyntaxes.Select(attributeSyntax =>
        {
            var attributeName = attributeSyntax.Name + "Attribute";

            return attributeName switch
            {
                nameof(RestApiAttribute) => ApiKind.Rest,
                nameof(GrpcApiAttribute) => ApiKind.Grpc,
                _ => throw new ArgumentException($"Unknown ApiKind: {attributeSyntax.Name.ToString()}")
            };
        }).ToList();

    public bool Equals(ModelSourceInfo? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return TypeName == other.TypeName && TypeNamespace == other.TypeNamespace &&
               NamedTypeSymbol.Name == other.NamedTypeSymbol.Name &&
               ApiKinds.OrderBy(a => a).SequenceEqual(other.ApiKinds.OrderBy(a => a));
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;

        return obj.GetType() == GetType() && Equals((ModelSourceInfo)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = TypeName.GetHashCode();

            hashCode = (hashCode * 397) ^ TypeNamespace.GetHashCode();
            hashCode = (hashCode * 397) ^ NamedTypeSymbol.Name.GetHashCode();
            hashCode = (hashCode * 397) ^ string.Join(string.Empty, ApiKinds.OrderBy(a => a)).GetHashCode();

            return hashCode;
        }
    }
}
