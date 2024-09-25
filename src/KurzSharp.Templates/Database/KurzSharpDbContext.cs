#if NET8_0_OR_GREATER
using KurzSharp.Templates.Models;
using Microsoft.EntityFrameworkCore;
#endif

// NOTE: Do not change namespace as it's referenced by string in `RestApiSourceGenerator`
// but isn't linked as 'Compiled' due to not being support for netstandard2.0
namespace KurzSharp.Templates.Database;

#if NET8_0_OR_GREATER
public partial class KurzSharpDbContext : DbContext
{
    public KurzSharpDbContext(DbContextOptions<KurzSharpDbContext> options) : base(options)
    {
    }

    public DbSet<PlaceholderModel> PlaceholderModels { get; set; }
}
#endif

// NOTE: Leave out class/interface declarations out of if NET8_0_OR_GREATER check to make it easier use in SourceGen with netstandard2.0
public partial class KurzSharpDbContext
{
}
