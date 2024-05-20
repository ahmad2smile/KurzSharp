#if NET7_0_OR_GREATER
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Routing;
using KurzSharp.Templates.Database;
using KurzSharp.Templates.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
#endif

// NOTE: Do not change namespace as it's referenced by string in `RestApiSourceGenerator`
// but isn't linked as 'Compiled' due to not being support for netstandard2.0
namespace KurzSharp.Templates;

public static class KurzSharpSetupExtension
{
#if NET7_0_OR_GREATER
    public static void AddKurzSharp(this IServiceCollection services)
    {
        AddKurzSharp(services, o => o.UseInMemoryDatabase("ProductsDb"));
    }

    public static void AddKurzSharp(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
    {
        services.AddDbContext<KurzSharpDbContext>(optionsAction);

        // NOTE: Register Models with DI so when new instance of those are requested from DI it would automatically provide
        // DI requested Deps in the model constructor ex: Logger.
        services.AddTransient<PlaceholderModel>();

        services.AddGrpc().AddJsonTranscoding();
        services.AddGrpcSwagger();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo() { Title = "gRPC transcoding", Version = "v1" });
            c.CustomSchemaIds(t => t.ToString());
        });
    }

    public static void MapKurzSharpServices(this IEndpointRouteBuilder builder)
    {
        builder.MapGrpcService<GrpcApi.PlaceholderModelGrpcService>();
    }
#endif
}
