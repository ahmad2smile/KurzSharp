using System.Collections.Immutable;
using System.Text;
using KurzSharp.Templates;
using KurzSharp.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using KurzSharp.Templates.RestApi;
using KurzSharp.Templates.Database;
using KurzSharp.Templates.GrpcApi;
using KurzSharp.Templates.Services;

namespace KurzSharp;

[Generator]
public class ApiSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var incrementalValueProvider = DeclarationsUtils.CreateSyntaxProvider<ClassDeclarationSyntax>(context);

        context.RegisterSourceOutput(incrementalValueProvider, Execute);
    }

    private static void Execute(SourceProductionContext ctx,
        (Compilation Left, ImmutableArray<(ClassDeclarationSyntax syntax, IEnumerable<AttributeSyntax> attributes)>
            Right) arg2)
    {
        var (compilation, classDeclarationsWithAttrs) = arg2;

        var modelSourceInfos = classDeclarationsWithAttrs.Select(attr =>
            {
                var (syntax, attributes) = attr;
                var symbol = compilation.GetSemanticModel(syntax.SyntaxTree).GetDeclaredSymbol(syntax);

                if (symbol is not INamedTypeSymbol typeSymbol)
                {
                    return null;
                }

                var typeName = syntax.Identifier.ToString();
                var typeNamespace = symbol.ContainingNamespace.ToDisplayString();

                return new ModelSourceInfo(typeName, typeNamespace, typeSymbol, attributes.ToList());
            })
            .Where(m => m != null)
            .Cast<ModelSourceInfo>()
            .ToImmutableHashSet()
            .ToList();

        AddServices(modelSourceInfos.ToList(), ctx);
        AddController(modelSourceInfos.Where(i => i.ApiKinds.Contains(ApiKind.Rest)).ToList(), ctx);
        AddGrpc(modelSourceInfos.Where(i => i.ApiKinds.Contains(ApiKind.Grpc)).ToList().ToList(), ctx);
        AddModelDto(modelSourceInfos.ToList(), ctx);
        AddPartialModel(modelSourceInfos.ToList(), ctx);
        AddDbContext(modelSourceInfos.ToList(), ctx);
        AddSetupExtension(modelSourceInfos.ToList(), ctx);
    }

    private static void AddServices(IList<ModelSourceInfo> modelSourceInfos, SourceProductionContext ctx)
    {
        var iServiceSrc = TemplatesUtils.GetTemplateFileContent("Services", nameof(IPlaceholderModelService));
        var serviceSrc = TemplatesUtils.GetTemplateFileContent("Services", nameof(PlaceholderModelService));

        foreach (var modelSourceInfo in modelSourceInfos)
        {
            var typeName = modelSourceInfo.TypeName;
            var typeNamespace = modelSourceInfo.TypeNamespace;

            var iService = iServiceSrc.FixupNamespaces(typeNamespace)
                .ReplacePlaceholderType(typeName)
                .AddUsing(typeNamespace);
            var service = serviceSrc.FixupNamespaces(typeNamespace)
                .ReplacePlaceholderType(typeName)
                .AddUsing(typeNamespace);

            ctx.AddSource($"I{typeName}Service.g.cs", SourceText.From(iService, Encoding.UTF8));
            ctx.AddSource($"{typeName}Service.g.cs", SourceText.From(service, Encoding.UTF8));
        }

        var serviceResultSrc = TemplatesUtils.GetTemplateFileContent("Services", nameof(ServiceResult<string>));
        var serviceResult = serviceResultSrc.FixupNamespaces(modelSourceInfos.First().TypeNamespace);
        ctx.AddSource($"ServiceResult.g.cs", SourceText.From(serviceResult, Encoding.UTF8));
    }

    private static void AddController(IList<ModelSourceInfo> modelSourceInfos, SourceProductionContext ctx)
    {
        var controllerSource =
            TemplatesUtils.GetTemplateFileContent("RestApi", nameof(PlaceholderModelController));

        foreach (var modelSourceInfo in modelSourceInfos)
        {
            var typeName = modelSourceInfo.TypeName;
            var typeNamespace = modelSourceInfo.TypeNamespace;

            var source = controllerSource.FixupNamespaces(typeNamespace)
                .ReplacePlaceholderType(typeName)
                .AddUsing(typeNamespace);

            ctx.AddSource($"{typeName}Controller.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private static void AddGrpc(IList<ModelSourceInfo> modelSourceInfos, SourceProductionContext ctx)
    {
        var iServiceSource = TemplatesUtils.GetTemplateFileContent("GrpcApi", nameof(IPlaceholderModelGrpcService));
        var serviceSource = TemplatesUtils.GetTemplateFileContent("GrpcApi", nameof(PlaceholderModelGrpcService));

        foreach (var modelSourceInfo in modelSourceInfos)
        {
            var typeName = modelSourceInfo.TypeName;
            var typeNamespace = modelSourceInfo.TypeNamespace;


            var iService = iServiceSource.FixupNamespaces(typeNamespace)
                .ReplacePlaceholderType(typeName)
                .AddUsing(typeNamespace);
            var service = serviceSource.FixupNamespaces(typeNamespace)
                .ReplacePlaceholderType(typeName)
                .AddUsing(typeNamespace);

            ctx.AddSource($"I{typeName}GrpcService.g.cs", SourceText.From(iService, Encoding.UTF8));
            ctx.AddSource($"{typeName}GrpcService.g.cs", SourceText.From(service, Encoding.UTF8));
        }
    }


    private static void AddModelDto(IList<ModelSourceInfo> modelSourceInfos, SourceProductionContext ctx)
    {
        foreach (var sourceInfo in modelSourceInfos)
        {
            var typeSymbol = sourceInfo.NamedTypeSymbol;
            var typeName = sourceInfo.TypeName;
            var typeNamespace = sourceInfo.TypeNamespace;

            var props = typeSymbol.GetMembers()
                .Where(s => s.Kind == SymbolKind.Property)
                .Cast<IPropertySymbol>()
                .ToArray();

            var propsDeclarations = string.Join("\n",
                props.Select((s, i) =>
                    $"[DataMember(Order = {i + 1})]\n{s.DeclaredAccessibility.ToString().ToLower()} {s.Type} {s.Name} {{ get; set; }}"));

            var propsMaps = string.Join("\n", props.Select(p => $"model.{p.Name} = {p.Name};"));

            var source = TemplatesUtils
                .GetTemplateFileContent("Models", $"{TemplatesUtils.PlaceholderTypeName}Dto")
                .FixupNamespaces(typeNamespace)
                .Replace("[DataMember(Order = 1)]", string.Empty)
                .Replace("public Guid PlaceholderId { get; set; }", propsDeclarations)
                .Replace("model.Id = PlaceholderId;", propsMaps)
                .ReplacePlaceholderType(typeName)
                .AddUsing(typeNamespace);

            var fileName = $"{TemplatesUtils.PlaceholderTypeName.ReplacePlaceholderType(typeName)}Dto";

            ctx.AddSource($"{fileName}.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private static void AddPartialModel(IList<ModelSourceInfo> modelSourceInfos, SourceProductionContext ctx)
    {
        foreach (var modelSourceInfo in modelSourceInfos)
        {
            var typeName = modelSourceInfo.TypeName;
            var typeNamespace = modelSourceInfo.TypeNamespace;

            var source = TemplatesUtils
                .GetTemplateFileContent("Models", TemplatesUtils.PlaceholderTypeName)
                .Replace("public Guid Id { get; set; }", string.Empty)
                .FixupNamespaces(typeNamespace)
                .ReplacePlaceholderType(typeName)
                .AddUsing(typeNamespace);

            var fileName = TemplatesUtils.PlaceholderTypeName.ReplacePlaceholderType(typeName);

            ctx.AddSource($"{fileName}.g.cs", SourceText.From(source, Encoding.UTF8));
        }

        var hooksSource = TemplatesUtils.GetTemplateFileContent("Hooks", nameof(LifecycleHooks<string>));
        ctx.AddSource($"{nameof(LifecycleHooks<string>)}.g.cs", SourceText.From(hooksSource, Encoding.UTF8));
    }

    private static void AddDbContext(IList<ModelSourceInfo> modelSourceInfos, SourceProductionContext ctx)
    {
        const string dbContextName = nameof(KurzSharpDbContext);

        var dbContextSource = TemplatesUtils.GetTemplateFileContent("Database", dbContextName);

        const string placeholderDbSet =
            $"public DbSet<{TemplatesUtils.PlaceholderDtoTypeName}> {TemplatesUtils.PlaceholderTypeName}s {{ get; set; }}";

        var dbSets = string.Join("\n",
            modelSourceInfos.Select(t => placeholderDbSet.ReplacePlaceholderType(t.TypeName)));

        var source = dbContextSource.Replace(placeholderDbSet, dbSets)
            // NOTE: Only the first namespace is used because only root namespace is required for this
            .FixupNamespaces(modelSourceInfos.First().TypeNamespace)
            .AddUsing(modelSourceInfos.Select(t => t.TypeNamespace));

        ctx.AddSource($"{dbContextName}.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static void AddSetupExtension(IList<ModelSourceInfo> modelSourceInfos, SourceProductionContext ctx)
    {
        const string extensionSetupFileName = nameof(KurzSharpSetupExtension);

        var setupExtSource = TemplatesUtils.GetTemplateFileContent(extensionSetupFileName);

        var modelInstanceServices = modelSourceInfos.Aggregate(string.Empty,
            (acc, m) => acc + $"services.AddSingleton<{m.NamedTypeSymbol.Name}>();\n");

        var modelServices = modelSourceInfos.Aggregate(string.Empty,
            (acc, m) => acc +
                        $"services.AddScoped<I{m.NamedTypeSymbol.Name}Service, {m.NamedTypeSymbol.Name}Service>();\n");

        var grpcModelSourceInfos = modelSourceInfos.Where(i => i.ApiKinds.Contains(ApiKind.Grpc)).ToList();

        // var modelGrpcClients = grpcModelSourceInfos.Aggregate(string.Empty,
        //     (acc, m) => acc +
        //                 $"services.AddCodeFirstGrpcClient<I{m.TypeName}Service>(o => {{ o.Address = clientBaseAddress; }});\n");
        var mapGrpcServices = grpcModelSourceInfos.Aggregate(string.Empty,
            (acc, m) => acc + $"builder.MapGrpcService<KurzSharp.GrpcApi.{m.TypeName}GrpcService>();\n");

        var source = setupExtSource.Replace("services.AddTransient<PlaceholderModel>();", modelInstanceServices)
            .Replace("services.AddScoped<IPlaceholderModelService, PlaceholderModelService>();", modelServices)
            // .Replace(
            //     "services.AddCodeFirstGrpcClient<IPlaceholderModelService>(o => { o.Address = clientBaseAddress; });",
            //     modelGrpcClients)
            .Replace("builder.MapGrpcService<GrpcApi.PlaceholderModelGrpcService>();", mapGrpcServices)
            // NOTE: Only the first namespace is used because only root namespace is required for this
            .FixupNamespaces(modelSourceInfos.First().TypeNamespace)
            .AddUsing(modelSourceInfos.Select(t => t.TypeNamespace));

        ctx.AddSource($"{extensionSetupFileName}.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
