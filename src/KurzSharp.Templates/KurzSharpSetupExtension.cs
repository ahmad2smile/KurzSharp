#if NET7_0_OR_GREATER

using KurzSharp.Templates.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// NOTE: Do not change namespace as it's referenced by string in `RestApiSourceGenerator`
// but isn't linked as 'Compiled' due to not being support for netstandard2.0
namespace KurzSharp.Templates;

public static class KurzSharpSetupExtension
{
    public static void AddKurzSharp(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
    {
        services.AddDbContext<KurzSharpDbContext>(optionsAction);
    }
}

#endif