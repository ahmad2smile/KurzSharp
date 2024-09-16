
using KurzSharp.Templates.Database;
using KurzSharp.Templates.Models;
#if NET8_0_OR_GREATER
using HotChocolate.Data;
using HotChocolate.Types;
#endif

namespace KurzSharp.Templates.GraphQlApi;

#if NET8_0_OR_GREATER
[ExtendObjectType(typeof(Query))]
#endif
public class PlaceholderModelQuery
{
#if NET8_0_OR_GREATER
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<PlaceholderModelDto> GetPlaceholderModels(KurzSharpDbContext context) => context.PlaceholderModels;
#endif
}
