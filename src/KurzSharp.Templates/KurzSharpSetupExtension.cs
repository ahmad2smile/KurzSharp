using KurzSharp.Templates.Database;
#if NET5_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
using Microsoft.Extensions.DependencyInjection;

// NOTE: Do not change namespace as it's referenced by string in `RestApiSourceGenerator`
// but isn't linked as 'Compiled' due to not being support for netstandard2.0
namespace KurzSharp.Templates;

public static class KurzSharpSetupExtension
{
#if NET5_0_OR_GREATER
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
    }
#endif
}
