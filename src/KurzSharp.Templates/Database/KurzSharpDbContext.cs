using Microsoft.EntityFrameworkCore;

// NOTE: Do not change namespace as it's referenced by string in `RestApiSourceGenerator`
// but isn't linked as 'Compiled' due to not being support for netstandard2.0
namespace KurzSharp.Templates.Database;

// NOTE: Do not change name as it's referenced by string in `RestApiSourceGenerator`
// but isn't linked as 'Compiled' due to not being support for netstandard2.0
public class KurzSharpDbContext: DbContext
{
    public KurzSharpDbContext(DbContextOptions<KurzSharpDbContext> options): base(options)
    {
    }
    
    public DbSet<PlaceholderModel> PlaceholderModels { get; set; }
}
