namespace KurzSharp.Templates.Models;

public static class PlaceholderModelExtensions
{
    public static PlaceholderModelDto ToDto(this PlaceholderModel model)
    {
        var dto = new PlaceholderModelDto();

#if NET8_0_OR_GREATER
        dto.PlaceholderId = model.Id;
#endif

        return dto;
    }

    public static IEnumerable<PlaceholderModelDto> ToDtos(this IEnumerable<PlaceholderModel> models) =>
        models.Select(m => m.ToDto());

    public static IQueryable<PlaceholderModelDto> ToDtos(this IQueryable<PlaceholderModel> models) =>
        models.Select(m => m.ToDto());
}
