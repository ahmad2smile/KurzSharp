using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using KurzSharp.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace KurzSharp;

[Generator]
public class RestApiSourceGenerator : IIncrementalGenerator
{
    private const string TemplatesNamespace = "KurzSharp.Templates";
    private const string RestApiTemplatesNamespace = $"{TemplatesNamespace}.RestApi";
    private const string DbTemplatesNamespace = $"{TemplatesNamespace}.Database";

    private const string PlaceholderTypeName = nameof(PlaceholderModel);

    // NOTE: Can't use `nameof` as that would error on NET7 code being in .netstandard 2.0 (Source Gen) Code
    private const string DbContextName = "KurzSharpDbContext";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarationsWithAttrsProvider =
            DeclarationsUtils.CreateSyntaxProvider<ClassDeclarationSyntax, RestApiAttribute>(context);
        var declarationsCombinedCompilation =
            context.CompilationProvider.Combine(classDeclarationsWithAttrsProvider.Collect());

        context.RegisterSourceOutput(declarationsCombinedCompilation, Execute);
    }

    private static void Execute(SourceProductionContext ctx,
        (Compilation Left, ImmutableArray<(ClassDeclarationSyntax syntax, AttributeSyntax attribute)> Right) arg2)
    {
        var (compilation, classDeclarationsWithAttrs) = arg2;

        var typesAndNamespaces = new List<(string type, string typeNamespace)>();

        foreach (var (syntax, _) in classDeclarationsWithAttrs)
        {
            var symbol = compilation.GetSemanticModel(syntax.SyntaxTree).GetDeclaredSymbol(syntax);

            if (symbol is null)
            {
                continue;
            }

            var typeName = syntax.Identifier.ToString();
            var typeNamespace = symbol.ContainingNamespace.ToDisplayString();

            typesAndNamespaces.Add((typeName, typeNamespace));

            AddController(typeName, typeNamespace, syntax, ctx);
        }

        AddDbContext(typesAndNamespaces, ctx);
        AddSetupExtension(typesAndNamespaces, ctx);
    }

    private static void AddController(string typeName, string typeNamespace, ClassDeclarationSyntax syntax,
        SourceProductionContext ctx)
    {
        var controllerName = $"{typeName}Controller";
        // NOTE: Can't use `nameof` as that would error on NET7 code being in .netstandard 2.0 (Source Gen) Code
        const string placeholderControllerName = $"{PlaceholderTypeName}Controller";

        var controllerSource =
            TemplatesUtils.GetTemplateFileContent(RestApiTemplatesNamespace, placeholderControllerName);

        var source = controllerSource.InjectHooks(syntax).ReplacePlaceholderType(typeName).FixupNamespaces()
            .AddUsing(typeNamespace);

        ctx.AddSource($"{controllerName}.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static void AddDbContext(IList<(string type, string typeNamespace)> typesAndNamespaces,
        SourceProductionContext ctx)
    {
        var typeNames = typesAndNamespaces.Select(t => t.type);
        var typeNamespaces = typesAndNamespaces.Select(t => t.typeNamespace);

        var dbContextSource =
            TemplatesUtils.GetTemplateFileContent(DbTemplatesNamespace, DbContextName);

        const string placeholderDbSet = $"public DbSet<{PlaceholderTypeName}> {PlaceholderTypeName}s {{ get; set; }}";

        var dbSets = string.Join("\n", typeNames.Select(t => placeholderDbSet.ReplacePlaceholderType(t)));

        var source = dbContextSource.Replace(placeholderDbSet, dbSets)
            .FixupNamespaces().AddUsing(typeNamespaces);

        ctx.AddSource($"{DbContextName}.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static void AddSetupExtension(
        IEnumerable<(string type, string typeNamespace)> typesAndNamespaces, SourceProductionContext ctx)
    {
        const string extensionSetupFileName = "KurzSharpSetupExtension";
        var setupExtSource = TemplatesUtils.GetTemplateFileContent(TemplatesNamespace, extensionSetupFileName);

        var source = setupExtSource.FixupNamespaces().AddUsing(typesAndNamespaces.Select(t => t.typeNamespace));

        ctx.AddSource($"{extensionSetupFileName}.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}