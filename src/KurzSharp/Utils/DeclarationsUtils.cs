using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KurzSharp.Utils;

public static class DeclarationsUtils
{
    private const string AttributeOptionalPostfix = "Attribute";

    public static
        IncrementalValueProvider<(Compilation Left, ImmutableArray<(T syntax, IEnumerable<AttributeSyntax> attribute)> Right)>
        CreateSyntaxProvider<T>(IncrementalGeneratorInitializationContext context) where T : TypeDeclarationSyntax
    {
        var attrs = new[]
        {
            nameof(RestApiAttribute),
            nameof(RestApiAttribute).Replace(AttributeOptionalPostfix, string.Empty),
            nameof(GrpcApiAttribute),
            nameof(GrpcApiAttribute).Replace(AttributeOptionalPostfix, string.Empty),
        };

        var classDeclarationsWithAttrsProvider = context.SyntaxProvider.CreateSyntaxProvider((node, _) =>
                node is T
                {
                    AttributeLists: { Count: >= 1 } attributes,
                } &&
                attributes.SelectMany(a => a.Attributes)
                    .Any(a => attrs.Contains(a.Name.ToString())),
            (syntaxContext, _) =>
            {
                var syntax = (T)syntaxContext.Node;

                // NOTE: Send through the actual Attribute to use some change Code Gen based on options take in Attr
                // e.g: [MyCustomAttribute("myOption1")] // "myOption1" can be used with this
                var attributes = syntax.AttributeLists.SelectMany(list => list.Attributes);

                return (syntax, attributes);
            });

        return context.CompilationProvider.Combine(classDeclarationsWithAttrsProvider.Collect());
    }
}
