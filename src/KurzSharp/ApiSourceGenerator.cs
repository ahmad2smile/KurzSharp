using System.Collections.Immutable;
using System.Text;
using KurzSharp.Templates;
using KurzSharp.Templates.AdminDashboard;
using KurzSharp.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using KurzSharp.Templates.RestApi;
using KurzSharp.Templates.Database;
using KurzSharp.Templates.GraphQlApi;
using KurzSharp.Templates.GrpcApi;
using KurzSharp.Templates.Models;
using KurzSharp.Templates.Services;

namespace KurzSharp;

[Generator]
public class ApiSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        const string attributeOptionalPostfix = "Attribute";
        var attrs = new[]
        {
            nameof(RestApiAttribute),
            nameof(RestApiAttribute).Replace(attributeOptionalPostfix, string.Empty),
            nameof(GrpcApiAttribute),
            nameof(GrpcApiAttribute).Replace(attributeOptionalPostfix, string.Empty),
            nameof(GraphQlApiAttribute),
            nameof(GraphQlApiAttribute).Replace(attributeOptionalPostfix, string.Empty),
            nameof(AdminDashboardAttribute),
            nameof(AdminDashboardAttribute).Replace(attributeOptionalPostfix, string.Empty),
        };

        var classDeclarationsWithAttrsProvider = context.SyntaxProvider.CreateSyntaxProvider((node, _) =>
                node is TypeDeclarationSyntax
                {
                    AttributeLists: { Count: >= 1 } attributes,
                } &&
                attributes.SelectMany(a => a.Attributes)
                    .Any(a => attrs.Contains(a.Name.ToString())),
            (syntaxContext, _) =>
            {
                var syntax = (ClassDeclarationSyntax)syntaxContext.Node;

                // NOTE: Send through the actual Attribute to use some change Code Gen based on options take in Attr
                // e.g: [MyCustomAttribute("myOption1")] // "myOption1" can be used with this
                var attributes = syntax.AttributeLists.SelectMany(list => list.Attributes);

                return (syntax, attributes);
            });

        var incrementalValueProvider =
            context.CompilationProvider.Combine(classDeclarationsWithAttrsProvider.Collect());

        context.RegisterSourceOutput(incrementalValueProvider, Execute);
    }

    private static void Execute(SourceProductionContext ctx,
        (Compilation Left, ImmutableArray<(ClassDeclarationSyntax syntax, IEnumerable<AttributeSyntax> attributes)>
            Right) arg2)
    {
        var (compilation, classDeclarationsWithAttrs) = arg2;

        var modelSourceInfos = new List<ModelSourceInfo>();
        // NOTE: A partial type split across files yields multiple syntax nodes for the same symbol;
        // dedupe so we don't generate (and collide on) the same hint name twice.
        var seenTypes = new HashSet<string>();

        foreach (var (syntax, attributes) in classDeclarationsWithAttrs)
        {
            var symbol = compilation.GetSemanticModel(syntax.SyntaxTree).GetDeclaredSymbol(syntax);

            if (symbol is not INamedTypeSymbol typeSymbol)
            {
                continue;
            }

            if (!syntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
            {
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.ModelMustBePartial,
                    syntax.Identifier.GetLocation(), syntax.Identifier.ToString()));
                continue;
            }

            if (!seenTypes.Add(typeSymbol.ToDisplayString()))
            {
                continue;
            }

            var typeName = syntax.Identifier.ToString();
            var typeNamespace = symbol.ContainingNamespace.ToDisplayString();

            var props = typeSymbol.GetMembers()
                .Where(s => s.Kind == SymbolKind.Property)
                .Cast<IPropertySymbol>()
                .Select(p => new ModelProperty(p.Name, p.Type.ToString()!,
                    p.DeclaredAccessibility.ToString().ToLower(), p.GetAttributes()))
                .ToList();

            if (props.Count == 0)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.ModelMustHaveProperties,
                    syntax.Identifier.GetLocation(), syntax.Identifier.ToString()));
                continue;
            }

            modelSourceInfos.Add(new ModelSourceInfo(typeName, typeNamespace, typeSymbol, attributes.ToList(), props));
        }

        AddServices(modelSourceInfos, ctx);
        AddAdminDashboard(modelSourceInfos.Where(m => m.ApiKinds.Contains(ApiKind.AdminDashboard)).ToArray(), ctx);
        AddModels(modelSourceInfos, ctx);
        AddDbContext(modelSourceInfos, ctx);
        AddSetupExtension(modelSourceInfos, ctx);
    }

    private static void AddServices(IList<ModelSourceInfo> modelSourceInfos, SourceProductionContext ctx)
    {
        if (modelSourceInfos.Count == 0)
        {
            return;
        }

        var iService = TemplatesUtils.GetFileContent(nameof(IPlaceholderModelService), "Services");
        var service = TemplatesUtils.GetFileContent(nameof(PlaceholderModelService), "Services");
        var controller = TemplatesUtils.GetFileContent(nameof(PlaceholderModelController), "RestApi");
        var iServiceGrpc = TemplatesUtils.GetFileContent(nameof(IPlaceholderModelGrpcService), "GrpcApi");
        var serviceGrpc = TemplatesUtils.GetFileContent(nameof(PlaceholderModelGrpcService), "GrpcApi");
        var queryGraphQl = TemplatesUtils.GetFileContent(nameof(PlaceholderModelQuery), "GraphQlApi");
        var mutationGraphQl = TemplatesUtils.GetFileContent(nameof(PlaceholderModelMutation), "GraphQlApi");

        foreach (var modelSourceInfo in modelSourceInfos)
        {
            var typ = modelSourceInfo.TypeName;

            ctx.AddSource($"I{typ}Service.g.cs",
                SourceText.From(iService.FixReferences(modelSourceInfo), Encoding.UTF8));
            ctx.AddSource($"{typ}Service.g.cs", SourceText.From(service.FixReferences(modelSourceInfo), Encoding.UTF8));

            if (modelSourceInfo.ApiKinds.Contains(ApiKind.Rest))
            {
                ctx.AddSource($"{typ}Controller.g.cs",
                    SourceText.From(controller.FixReferences(modelSourceInfo), Encoding.UTF8));
            }

            if (modelSourceInfo.ApiKinds.Contains(ApiKind.Grpc))
            {
                ctx.AddSource($"I{typ}GrpcService.g.cs",
                    SourceText.From(iServiceGrpc.FixReferences(modelSourceInfo), Encoding.UTF8));
                ctx.AddSource($"{typ}GrpcService.g.cs",
                    SourceText.From(serviceGrpc.FixReferences(modelSourceInfo), Encoding.UTF8));
            }

            if (modelSourceInfo.ApiKinds.Contains(ApiKind.GraphQl))
            {
                ctx.AddSource($"{modelSourceInfo.TypeName}Query.g.cs",
                    SourceText.From(queryGraphQl.FixReferences(modelSourceInfo), Encoding.UTF8));
                ctx.AddSource($"{modelSourceInfo.TypeName}Mutation.g.cs",
                    SourceText.From(mutationGraphQl.FixReferences(modelSourceInfo), Encoding.UTF8));
            }
        }

        var firstModelSource = modelSourceInfos.First();

        var baseQuery = TemplatesUtils.GetFileContent(nameof(Query), "GraphQlApi")
            .FixReferences(firstModelSource);

        var baseMutation = TemplatesUtils.GetFileContent(nameof(Mutation), "GraphQlApi")
            .FixReferences(firstModelSource);

        var graphQlExceptions = TemplatesUtils.GetFileContent(nameof(GraphQlExceptions), "GraphQlApi")
            .FixReferences(firstModelSource);

        var grpcIdRequest = TemplatesUtils.GetFileContent(nameof(IdRequest), "GrpcApi")
            .FixReferences(firstModelSource);

        ctx.AddSource($"{nameof(Query)}.g.cs", SourceText.From(baseQuery, Encoding.UTF8));
        ctx.AddSource($"{nameof(Mutation)}.g.cs", SourceText.From(baseMutation, Encoding.UTF8));
        ctx.AddSource($"{nameof(GraphQlExceptions)}.g.cs", SourceText.From(graphQlExceptions, Encoding.UTF8));
        ctx.AddSource($"{nameof(IdRequest)}.g.cs", SourceText.From(grpcIdRequest, Encoding.UTF8));
    }

    private static void AddAdminDashboard(IList<ModelSourceInfo> modelSourceInfos, SourceProductionContext ctx)
    {
        if (modelSourceInfos.Count == 0)
        {
            return;
        }

        var placeholderModelComponent =
            TemplatesUtils.GetFileContent(nameof(PlaceholderModelComponent), "AdminDashboard");

        foreach (var modelSourceInfo in modelSourceInfos)
        {
            var typ = modelSourceInfo.TypeName;

            var modelComponentSrc = placeholderModelComponent.Replace(
                "AddColumn(nameof(PlaceholderModel.Id), \"Id\", columnsBuilder, ref seq2);",
                modelSourceInfo.Properties.Aggregate(string.Empty,
                    (x, p) => x +
                              $"AddColumn(nameof(PlaceholderModel.{p.Name}), \"{p.Name}\", columnsBuilder, ref seq2)\n;"));

            ctx.AddSource($"{typ}Component.g.cs",
                SourceText.From(modelComponentSrc.FixReferences(modelSourceInfo), Encoding.UTF8));
        }

        var firstModelSource = modelSourceInfos.First();

        var appComponent = TemplatesUtils.GetFileContent(nameof(AppComponent), "AdminDashboard")
            .FixReferences(firstModelSource);
        var homeComponent = TemplatesUtils.GetFileContent(nameof(HomeComponent), "AdminDashboard")
            .FixReferences(firstModelSource);
        var mainLayout = TemplatesUtils.GetFileContent(nameof(MainLayout), "AdminDashboard")
            .FixReferences(firstModelSource);
        var routesComponent = TemplatesUtils.GetFileContent(nameof(RoutesComponent), "AdminDashboard")
            .FixReferences(firstModelSource);

        var navMenuComponent = TemplatesUtils.GetFileContent(nameof(NavMenuComponent), "AdminDashboard")
            .Replace("CreateNavLink(builder2, \"placeholderModel\", \"PlaceHolderModel\");",
                modelSourceInfos.Aggregate(string.Empty,
                    (current, i) =>
                        current +
                        $"CreateNavLink(builder2, \"{i.TypeName.ToCamelCase()}s\", \"{i.TypeName}s\");\n"))
            .FixReferences(firstModelSource);

        ctx.AddSource($"{nameof(AppComponent)}.g.cs", SourceText.From(appComponent, Encoding.UTF8));
        ctx.AddSource($"{nameof(HomeComponent)}.g.cs", SourceText.From(homeComponent, Encoding.UTF8));
        ctx.AddSource($"{nameof(MainLayout)}.g.cs", SourceText.From(mainLayout, Encoding.UTF8));
        ctx.AddSource($"{nameof(NavMenuComponent)}.g.cs", SourceText.From(navMenuComponent, Encoding.UTF8));
        ctx.AddSource($"{nameof(RoutesComponent)}.g.cs", SourceText.From(routesComponent, Encoding.UTF8));
    }

    private static void AddModels(IList<ModelSourceInfo> modelSourceInfos, SourceProductionContext ctx)
    {
        foreach (var modelSourceInfo in modelSourceInfos)
        {
            var typeName = modelSourceInfo.TypeName;

            var propsDeclarations = string.Join("\n", modelSourceInfo.Properties
                .Select((p, i) =>
                    $$"""
                      {{TemplatesUtils.GetPropAttrSrc(p)}}
                      [DataMember(Order = {{i + 1}})]
                      {{p.AccessModifier}} {{p.Type}} {{p.Name}} { get; set; }
                      """));

            var modelDto = TemplatesUtils.GetFileContent(nameof(PlaceholderModelDto), "Models")
                .Replace("public Guid PlaceholderId { get; set; }", propsDeclarations)
                .Replace("model.Id = PlaceholderId;",
                    string.Join("\n", modelSourceInfo.Properties.Select(s => $"model.{s.Name} = {s.Name};\n")))
                .FixReferences(modelSourceInfo);

            var projection = string.Join(", ",
                modelSourceInfo.Properties.Select(p => $"{p.Name} = m.{p.Name}"));

            var modelExt = TemplatesUtils.GetFileContent(nameof(PlaceholderModelExtensions), "Models")
                .Replace("dto.PlaceholderId = model.Id;",
                    string.Join("\n", modelSourceInfo.Properties.Select(s => $"dto.{s.Name} = model.{s.Name};\n")))
                // EF Core cannot translate a method-call projection (m.ToDto()); use an object
                // initializer so the projection is translated to SQL for IQueryable sources.
                .Replace("m => m.ToDto()", $"m => new PlaceholderModelDto {{ {projection} }}")
                .FixReferences(modelSourceInfo);

            var modelSrc = TemplatesUtils.GetFileContent(nameof(PlaceholderModel), "Models")
                .Replace("public Guid Id { get; set; }", string.Empty);

            // NOTE: Declared == Explicitly declared in source code, not compiler generated.
            // If the user already declares a parameterless constructor, drop the generated one to
            // avoid a duplicate-constructor compile error. Otherwise keep the public
            // [JsonConstructor] so EF Core, the DTO `ToModel()` mapping and System.Text.Json can
            // all instantiate the model.
            var hasUserParameterlessCtor = modelSourceInfo.NamedTypeSymbol.InstanceConstructors.Any(x =>
                !x.IsImplicitlyDeclared && x.Parameters.Length == 0);

            if (hasUserParameterlessCtor)
            {
                modelSrc = modelSrc.Replace("[JsonConstructor]\n    public PlaceholderModel()\n    {\n    }",
                    string.Empty);
            }

            modelSrc = modelSrc.FixReferences(modelSourceInfo);

            ctx.AddSource($"{typeName}.g.cs", SourceText.From(modelSrc, Encoding.UTF8));
            ctx.AddSource($"{typeName}Dto.g.cs", SourceText.From(modelDto, Encoding.UTF8));
            ctx.AddSource($"{typeName}Extensions.g.cs", SourceText.From(modelExt, Encoding.UTF8));
        }

        var hooksSource = TemplatesUtils.GetFileContent(nameof(LifecycleHooks<string>), "Hooks");
        ctx.AddSource($"{nameof(LifecycleHooks<string>)}.g.cs", SourceText.From(hooksSource, Encoding.UTF8));
    }

    private static void AddDbContext(IList<ModelSourceInfo> modelSourceInfos, SourceProductionContext ctx)
    {
        const string dbContextName = nameof(KurzSharpDbContext);
        const string placeholderTypeName = nameof(PlaceholderModel);

        const string placeholderDbSet = $"public DbSet<{placeholderTypeName}> {placeholderTypeName}s {{ get; set; }}";

        var dbSets = string.Join("\n", modelSourceInfos.Select(placeholderDbSet.FixReferences));

        var source = TemplatesUtils.GetFileContent(dbContextName, "Database").Replace(placeholderDbSet, dbSets);
        source = modelSourceInfos.Aggregate(source, (acc, m) => acc.FixReferences(m));

        ctx.AddSource($"{dbContextName}.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static void AddSetupExtension(IList<ModelSourceInfo> modelSourceInfos, SourceProductionContext ctx)
    {
        var setupExtSource = TemplatesUtils.GetFileContent(nameof(KurzSharpSetupExtension));

        // NOTE: Models host the lifecycle hooks and may carry per-request mutable state, so they
        // must not be shared across requests; register them Scoped rather than Singleton.
        var modelInstanceServices = modelSourceInfos.Aggregate(string.Empty,
            (acc, m) => acc + $"services.AddScoped<{m.NamedTypeSymbol.Name}>();\n");

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
            .Replace("ADMIN_DASHBOARD",
                modelSourceInfos.Any(i => i.ApiKinds.Contains(ApiKind.AdminDashboard)) ? "true" : "false")
            .Replace("services.AddTransient<PlaceholderModel>();", modelInstanceServices)
            .Replace("services.AddScoped<IPlaceholderModelService, PlaceholderModelService>();", modelServices)
            .Replace(".AddTypeExtension<PlaceholderModelQuery>()", graphqlQueries)
            .Replace(".AddTypeExtension<PlaceholderModelMutation>()", graphqlMutations)
            .Replace("builder.MapGrpcService<GrpcApi.PlaceholderModelGrpcService>();", mapGrpcServices);
        source = modelSourceInfos.Aggregate(source, (acc, m) => acc.FixReferences(m));

        ctx.AddSource($"{nameof(KurzSharpSetupExtension)}.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
