#if NET7_0_OR_GREATER

using KurzSharp.Templates.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KurzSharp.Templates;

public static class KurzSharpSetupExtension
{
    public static void AddKurzSharp(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
    {
        services.AddDbContext<KurzSharpDbContext>(optionsAction);
    }
}

#endif