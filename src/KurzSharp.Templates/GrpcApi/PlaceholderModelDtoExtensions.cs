namespace KurzSharp.GrpcApi;

public static class PlaceholderModelDtoExtensions
{
    public static KurzSharp.Templates.Models.PlaceholderModelDto ToModel(this PlaceholderModelDto dto) => new()
    {
        // Props Map Dto
    };

    public static PlaceholderModelDto FromModel(this KurzSharp.Templates.Models.PlaceholderModelDto dto) => new()
    {
        // Props Map Model
    };
}
