using System.Collections.Immutable;
using System.Text;
using KurzSharp.Templates;
using KurzSharp.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using KurzSharp.Templates.RestApi;
using KurzSharp.Templates.Database;
using KurzSharp.Templates.GraphQlApi;
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

        AddServices(modelSourceInfos, ctx);

        if (modelSourceInfos.Where(i => i.ApiKinds.Contains(ApiKind.Rest)).ToList() is { Count: > 0 } rests)
        {
            AddController(rests, ctx);
        }

        if (modelSourceInfos.Where(i => i.ApiKinds.Contains(ApiKind.Grpc)).ToList() is { Count: > 0 } grpcs)
        {
            AddGrpc(grpcs, ctx);
        }

        if (modelSourceInfos.Where(i => i.ApiKinds.Contains(ApiKind.GraphQl)).ToList() is { Count: > 0 } graphqls)
        {
            AddGraphql(graphqls, ctx);
        }

        AddModelDto(modelSourceInfos, ctx);
        AddPartialModel(modelSourceInfos, ctx);
        AddDbContext(modelSourceInfos, ctx);
        AddSetupExtension(modelSourceInfos, ctx);
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
        var serviceResult = serviceResultSrc.FixupNamespaces(modelSourceInfos.FirstOrDefault()?.TypeNamespace ??
                                                             throw new InvalidOperationException(
                                                                 $"No type namespace found {nameof(AddServices)}"));
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

    private static void AddGraphql(IList<ModelSourceInfo> modelSourceInfos, SourceProductionContext ctx)
    {
        var modelQuerySource = TemplatesUtils.GetTemplateFileContent("GraphQlApi", nameof(PlaceholderModelQuery));
        var modelMutationSource = TemplatesUtils.GetTemplateFileContent("GraphQlApi", nameof(PlaceholderModelMutation));

        foreach (var modelSourceInfo in modelSourceInfos)
        {
            var typeName = modelSourceInfo.TypeName;
            var typeNamespace = modelSourceInfo.TypeNamespace;

            var modelQuery = modelQuerySource.FixupNamespaces(typeNamespace)
                .ReplacePlaceholderType(typeName)
                .AddUsing(typeNamespace);

            var modelMutation = modelMutationSource.FixupNamespaces(typeNamespace)
                .ReplacePlaceholderType(typeName)
                .AddUsing(typeNamespace);

            ctx.AddSource($"{typeName}Query.g.cs", SourceText.From(modelQuery, Encoding.UTF8));
            ctx.AddSource($"{typeName}Mutation.g.cs", SourceText.From(modelMutation, Encoding.UTF8));
        }

        var basesQuerySource = TemplatesUtils.GetTemplateFileContent("GraphQlApi", nameof(Query))
            .FixupNamespaces(modelSourceInfos.FirstOrDefault()?.TypeNamespace ??
                             throw new InvalidOperationException($"No type namespace in {nameof(AddGraphql)}."));

        var basesMutationSource = TemplatesUtils.GetTemplateFileContent("GraphQlApi", nameof(Mutation))
            .FixupNamespaces(modelSourceInfos.FirstOrDefault()?.TypeNamespace ??
                             throw new InvalidOperationException($"No type namespace in {nameof(AddGraphql)}."));

        ctx.AddSource("Query.g.cs", SourceText.From(basesQuerySource, Encoding.UTF8));
        ctx.AddSource("Mutation.g.cs", SourceText.From(basesMutationSource, Encoding.UTF8));
    }

    private static void AddModelDto(IList<ModelSourceInfo> modelSourceInfos, SourceProductionContext ctx)
    {
        foreach (var sourceInfo in modelSourceInfos)
        {
            var typeName = sourceInfo.TypeName;
            var typeNamespace = sourceInfo.TypeNamespace;

            var propsDeclarations = string.Join("\n",
                sourceInfo.NamedTypeSymbol
                    .GetMembers()
                    .Where(s => s.Kind == SymbolKind.Property)
                    .Cast<IPropertySymbol>()
                    .Select((s, i) =>
                        $$"""
                          {{GetPropertyAttributes(s.GetAttributes())}}
                          [DataMember(Order = {{i + 1}})]
                          {{s.DeclaredAccessibility.ToString().ToLower()}} {{s.Type}} {{s.Name}} { get; set; }
                          """));

            var source = TemplatesUtils.GetTemplateFileContent("Models", TemplatesUtils.PlaceholderDtoTypeName)
                .FixupNamespaces(typeNamespace)
                .Replace("[DataMember(Order = 1)]\n", string.Empty)
                .Replace("public Guid PlaceholderId { get; set; }", propsDeclarations)
                .ReplacePlaceholderType(typeName)
                .AddUsing(typeNamespace);

            var fileName = $"{TemplatesUtils.PlaceholderTypeName.ReplacePlaceholderType(typeName)}Dto";

            ctx.AddSource($"{fileName}.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private static string GetPropertyAttributes(ImmutableArray<AttributeData> attributes)
    {
        return attributes.Aggregate("\n", (syntax, attributeData) =>
        {
            var namedArgs = attributeData.NamedArguments.Select((namedArg) => $"{namedArg.Key}={namedArg.Value.Value}");
            var constructorArgs = attributeData.ConstructorArguments.Select(b => b.Value?.ToString());

            var args = string.Join(", ", namedArgs.Concat(constructorArgs));

            return syntax + $"[{attributeData.AttributeClass}({args})]";
        });
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

            // NOTE: Declared == Explicitly declared in source code, not compiler generated
            var hasDeclaredPublicCtor = modelSourceInfo.NamedTypeSymbol.InstanceConstructors.Any(x =>
                !x.IsImplicitlyDeclared && x.DeclaredAccessibility == Accessibility.Public);

            if (!hasDeclaredPublicCtor)
            {
                source = source.Replace("[JsonConstructor]\n    private", "[JsonConstructor]\n    public");
            }

            var fileName = TemplatesUtils.PlaceholderTypeName.ReplacePlaceholderType(typeName);

            ctx.AddSource($"{fileName}.g.cs", SourceText.From(source, Encoding.UTF8));
        }

        var hooksSource = TemplatesUtils.GetTemplateFileContent("Hooks", nameof(LifecycleHooks<string>));
        ctx.AddSource($"{nameof(LifecycleHooks<string>)}.g.cs", SourceText.From(hooksSource, Encoding.UTF8));
    }

    private static void AddDbContext(IList<ModelSourceInfo> modelSourceInfos, SourceProductionContext ctx)
    {
        const string dbContextName = nameof(KurzSharpDbContext);

        const string placeholderDbSet =
            $"public DbSet<{TemplatesUtils.PlaceholderDtoTypeName}> {TemplatesUtils.PlaceholderTypeName}s {{ get; set; }}";

        var dbSets = string.Join("\n",
            modelSourceInfos.Select(t => placeholderDbSet.ReplacePlaceholderType(t.TypeName)));

        var source = TemplatesUtils.GetTemplateFileContent("Database", dbContextName)
            .Replace(placeholderDbSet, dbSets)
            // NOTE: Only the first namespace is used because only root namespace is required for this
            .FixupNamespaces(modelSourceInfos.FirstOrDefault()?.TypeNamespace ??
                             throw new InvalidOperationException($"No type namespace in {nameof(AddDbContext)}."))
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

        var mapGrpcServices = modelSourceInfos.Where(i => i.ApiKinds.Contains(ApiKind.Grpc))
            .Aggregate(string.Empty,
                (acc, m) => acc + $"builder.MapGrpcService<KurzSharp.GrpcApi.{m.TypeName}GrpcService>();\n");

        var graphqlQueries = modelSourceInfos.Where(i => i.ApiKinds.Contains(ApiKind.GraphQl))
            .Aggregate(string.Empty, (acc, m) => acc + $".AddTypeExtension<{m.TypeName}Query>()\n");

        var graphqlMutations = modelSourceInfos.Where(i => i.ApiKinds.Contains(ApiKind.GraphQl))
            .Aggregate(string.Empty, (acc, m) => acc + $".AddTypeExtension<{m.TypeName}Mutation>()\n");


        var source = setupExtSource
            .Replace("REST_API", modelSourceInfos.Any(i => i.ApiKinds.Contains(ApiKind.Rest)) ? "true" : "false")
            .Replace("GRPC_API", modelSourceInfos.Any(i => i.ApiKinds.Contains(ApiKind.Grpc)) ? "true" : "false")
            .Replace("GRAPHQL_API", modelSourceInfos.Any(i => i.ApiKinds.Contains(ApiKind.GraphQl)) ? "true" : "false")
            .Replace("services.AddTransient<PlaceholderModel>();", modelInstanceServices)
            .Replace("services.AddScoped<IPlaceholderModelService, PlaceholderModelService>();", modelServices)
            .Replace(".AddTypeExtension<PlaceholderModelQuery>()", graphqlQueries)
            .Replace(".AddTypeExtension<PlaceholderModelMutation>()", graphqlMutations)
            .Replace("builder.MapGrpcService<GrpcApi.PlaceholderModelGrpcService>();", mapGrpcServices)
            // NOTE: Only the first namespace is used because only root namespace is required for this
            .FixupNamespaces(modelSourceInfos.FirstOrDefault()?.TypeNamespace ??
                             throw new InvalidOperationException($"No type namespace in {nameof(AddSetupExtension)}."))
            .AddUsing(modelSourceInfos.Select(t => t.TypeNamespace));

        ctx.AddSource($"{extensionSetupFileName}.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
