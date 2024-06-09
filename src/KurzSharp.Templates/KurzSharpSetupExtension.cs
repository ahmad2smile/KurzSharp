#if NET8_0_OR_GREATER
using ProtoBuf.Grpc.Server;
using Microsoft.AspNetCore.Routing;
using KurzSharp.Templates.Database;
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
        services.AddDbContext<KurzSharpDbContext>(optionsAction);

        // NOTE: Register Models with DI so when new instance of those are requested from DI it would automatically provide
        // DI requested Deps in the model constructor ex: Logger.
        services.AddTransient<PlaceholderModel>();
        services.AddScoped<IPlaceholderModelService, PlaceholderModelService>();

        services.AddCodeFirstGrpc();
    }

    public static void MapKurzSharpServices(this IEndpointRouteBuilder builder)
    {
        builder.MapGrpcService<GrpcApi.PlaceholderModelGrpcService>();
    }
#endif
}
