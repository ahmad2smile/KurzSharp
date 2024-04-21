using System.Collections.Immutable;
using System.Text;
using KurzSharp.Templates;
using KurzSharp.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using KurzSharp.Templates.RestApi;
using KurzSharp.Templates.Database;

namespace KurzSharp;

[Generator]
public class RestApiSourceGenerator : IIncrementalGenerator
{
    private const string TemplatesNamespace = "KurzSharp.Templates";

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

        var typesAndNamespaces = new List<ModelSourceInfo>();
        var namedTypeSymbols = new List<INamedTypeSymbol>();

        foreach (var (syntax, _) in classDeclarationsWithAttrs)
        {
            var symbol = compilation.GetSemanticModel(syntax.SyntaxTree).GetDeclaredSymbol(syntax);

            if (symbol is not INamedTypeSymbol typeSymbol)
            {
                continue;
            }

            namedTypeSymbols.Add(typeSymbol);
            var typeName = syntax.Identifier.ToString();
            var typeNamespace = symbol.ContainingNamespace.ToDisplayString();

            typesAndNamespaces.Add(new ModelSourceInfo(typeName, typeNamespace, typeSymbol));

            AddModelDto(typeName, typeNamespace, typeSymbol, ctx);
            AddPartialModel(typeName, typeNamespace, ctx);
            AddController(typeName, typeNamespace, ctx);
        }

        AddDbContext(typesAndNamespaces, ctx);
        AddSetupExtension(typesAndNamespaces, namedTypeSymbols, ctx);
    }

    private static void AddController(string typeName, string typeNamespace, SourceProductionContext ctx)
    {
        var controllerSource =
            TemplatesUtils.GetTemplateFileContent($"{TemplatesNamespace}.RestApi", nameof(PlaceholderModelController));

        var source = controllerSource.FixupNamespaces()
            .ReplacePlaceholderType(typeName)
            .AddUsing(typeNamespace);

        ctx.AddSource($"{typeName}Controller.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static void AddModelDto(string typeName, string typeNamespace, INamespaceOrTypeSymbol typeSymbol,
        SourceProductionContext ctx)
    {
        var props = typeSymbol.GetMembers()
            .Where(s => s.Kind == SymbolKind.Property)
            .Cast<IPropertySymbol>()
            .ToArray();

        var propsDeclarations = string.Join("\n",
            props.Select(s => $"{s.DeclaredAccessibility.ToString().ToLower()} {s.Type} {s.Name} {{ get; set; }}"));

        var propsMaps = string.Join("\n", props.Select(p => $"model.{p.Name} = {p.Name};"));

        var source = TemplatesUtils
            .GetTemplateFileContent($"{TemplatesNamespace}.Models", $"{TemplatesUtils.PlaceholderTypeName}Dto")
            .Replace(TemplatesNamespace, typeNamespace)
            .Replace("public Guid PlaceholderId { get; set; }", propsDeclarations)
            .Replace("model.Id = PlaceholderId;", propsMaps)
            .ReplacePlaceholderType(typeName)
            .AddUsing(typeNamespace);

        var fileName = $"{TemplatesUtils.PlaceholderTypeName.ReplacePlaceholderType(typeName)}Dto";

        ctx.AddSource($"{fileName}.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static void AddPartialModel(string typeName, string typeNamespace, SourceProductionContext ctx)
    {
        var source = TemplatesUtils
            .GetTemplateFileContent($"{TemplatesNamespace}.Models", TemplatesUtils.PlaceholderTypeName)
            .Replace("public Guid Id { get; set; }", string.Empty)
            .Replace(TemplatesNamespace, typeNamespace)
            .ReplacePlaceholderType(typeName)
            .AddUsing(typeNamespace);

        var fileName = TemplatesUtils.PlaceholderTypeName.ReplacePlaceholderType(typeName);

        ctx.AddSource($"{fileName}.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static void AddDbContext(IList<ModelSourceInfo> modelSourceInfos, SourceProductionContext ctx)
    {
        const string dbTemplatesNamespace = $"{TemplatesNamespace}.Database";
        const string dbContextName = nameof(KurzSharpDbContext);

        var dbContextSource = TemplatesUtils.GetTemplateFileContent(dbTemplatesNamespace, dbContextName);

        const string placeholderDbSet =
            $"public DbSet<{TemplatesUtils.PlaceholderDtoTypeName}> {TemplatesUtils.PlaceholderTypeName}s {{ get; set; }}";

        var dbSets = string.Join("\n",
            modelSourceInfos.Select(t => placeholderDbSet.ReplacePlaceholderType(t.TypeName)));

        var source = dbContextSource.Replace(placeholderDbSet, dbSets)
            .FixupNamespaces()
            .AddUsing(modelSourceInfos.Select(t => t.TypeNamespace));

        ctx.AddSource($"{dbContextName}.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static void AddSetupExtension(IEnumerable<ModelSourceInfo> modelSourceInfos, List<INamedTypeSymbol> syntax,
        SourceProductionContext ctx)
    {
        const string extensionSetupFileName = nameof(KurzSharpSetupExtension);

        var setupExtSource = TemplatesUtils.GetTemplateFileContent(TemplatesNamespace, extensionSetupFileName);

        var registeredTypes = syntax.Aggregate(string.Empty,
            (acc, typeSymbol) => acc + $"services.AddSingleton<{typeSymbol.Name}>();");

        var source = setupExtSource.Replace("services.AddTransient<PlaceholderModel>();", registeredTypes)
            .FixupNamespaces()
            .AddUsing(modelSourceInfos.Select(t => t.TypeNamespace));

        ctx.AddSource($"{extensionSetupFileName}.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
