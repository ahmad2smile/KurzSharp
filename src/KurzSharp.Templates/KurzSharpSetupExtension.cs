#if NET8_0_OR_GREATER
using ProtoBuf.Grpc.Server;
using Microsoft.AspNetCore.Routing;
using KurzSharp.Templates.Database;
#if GRAPHQL_API
using KurzSharp.Templates.GraphQlApi;
using HotChocolate.Data;
#endif
using KurzSharp.Templates.Models;
using KurzSharp.Templates.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
#endif

// NOTE: Do not change namespace as it's referenced by string in `RestApiSourceGenerator`
// but isn't linked as 'Compiled' due to not being support for netstandard2.0
namespace KurzSharp.Templates;

public static class KurzSharpSetupExtension
{
#if NET8_0_OR_GREATER
    public static void AddKurzSharp(this IServiceCollection services)
    {
        AddKurzSharp(services, o => o.UseInMemoryDatabase("KurzSharpDb"));
    }

    public static void AddKurzSharp(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
    {
        // NOTE: Register Models with DI so when new instance of those are requested from DI it would automatically provide
        // DI requested Deps in the model constructor ex: Logger.
        services.AddTransient<PlaceholderModel>();
        services.AddPooledDbContextFactory<KurzSharpDbContext>(optionsAction);

#if REST_API || GRPC_API
        services.AddScoped<IPlaceholderModelService, PlaceholderModelService>();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
#endif

#if GRPC_API
        services.AddCodeFirstGrpc();
#endif

#if GRAPHQL_API
        services.AddGraphQLServer()
            .RegisterDbContext<KurzSharpDbContext>(DbContextKind.Pooled)
            .AddQueryType<Query>()
            .AddTypeExtension<PlaceholderModelQuery>()
            .AddMutationType<Mutation>()
            .AddTypeExtension<PlaceholderModelMutation>()
            .AddFiltering()
            .AddSorting()
            .AddProjections();
#endif
    }

    public static void MapKurzSharpServices(this WebApplication builder, bool mapSwaggerUi = true,
        bool mapControllers = true)
    {
#if REST_API || GRPC_API
        if (mapSwaggerUi)
        {
            builder.UseSwagger();
            builder.UseSwaggerUI();
        }
#endif

#if REST_API
        if (mapControllers)
        {
            builder.MapControllers();
        }
#endif

#if GRPC_API
        builder.MapGrpcService<GrpcApi.PlaceholderModelGrpcService>();
#endif

#if GRAPHQL_API
        builder.MapGraphQL();
#endif
    }
#endif
}
