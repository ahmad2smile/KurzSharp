namespace KurzSharp.Templates;

#if NET5_0_OR_GREATER
public partial class PlaceholderModelDto
{
    public Guid PlaceholderId { get; set; }

    public PlaceholderModel CopyToModel(PlaceholderModel model)
    {
        model.Id = PlaceholderId;

        return model;
    }
}
#endif

public partial class PlaceholderModelDto
{
}
