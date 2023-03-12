using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KurzSharp.Utils;

public class DeclarationsUtils
{
    private const string AttributeOptionalPostfix = "Attribute";

    public static IncrementalValuesProvider<(T syntax, AttributeSyntax attribute)>
        CreateSyntaxProvider<T, TAttribute>(IncrementalGeneratorInitializationContext context)
        where T : TypeDeclarationSyntax
    {
        var attrName = typeof(TAttribute).Name;
        var attrNameShort = attrName.Replace(AttributeOptionalPostfix, "");

        return context.SyntaxProvider.CreateSyntaxProvider((node, _) =>
                node is T
                {
                    AttributeLists: {Count: >= 1} attributes,
                } && attributes.SelectMany(a => a.Attributes).Any(a =>
                    a.Name.ToString() == attrName || a.Name.ToString() == attrNameShort),
            (syntaxContext, _) =>
            {
                var syntax = (T) syntaxContext.Node;

                // NOTE: Send through the actual Attribute to use some change Code Gen based on options take in Attr
                // e.g: [MyCustomAttribute("myOption1")] // "myOption1" can be used with this
                var attribute = syntax.AttributeLists.SelectMany(list => list.Attributes).FirstOrDefault();

                return (syntax, attribute);
            });
    }
}