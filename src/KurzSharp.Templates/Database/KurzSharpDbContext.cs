#if NET5_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif

// NOTE: Do not change namespace as it's referenced by string in `RestApiSourceGenerator`
// but isn't linked as 'Compiled' due to not being support for netstandard2.0
namespace KurzSharp.Templates.Database;

#if NET5_0_OR_GREATER
// NOTE: Do not change name as it's referenced by string in `RestApiSourceGenerator`
// but isn't linked as 'Compiled' due to not being support for netstandard2.0
public partial class KurzSharpDbContext : DbContext
{
    public KurzSharpDbContext(DbContextOptions<KurzSharpDbContext> options) : base(options)
    {
    }

    public DbSet<PlaceholderModelDto> PlaceholderModels { get; set; }
}
#endif

public partial class KurzSharpDbContext
{
}
