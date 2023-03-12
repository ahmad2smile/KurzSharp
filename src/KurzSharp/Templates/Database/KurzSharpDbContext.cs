#if NET7_0_OR_GREATER


using Microsoft.EntityFrameworkCore;

namespace KurzSharp.Templates.Database;

public class KurzSharpDbContext: DbContext
{
    public KurzSharpDbContext(DbContextOptions<KurzSharpDbContext> options): base(options)
    {
    }
    
    public DbSet<PlaceholderModel> PlaceholderModels { get; set; }
}

#endif
