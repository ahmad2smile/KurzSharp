using Microsoft.CodeAnalysis;

namespace KurzSharp.Utils;

public class ModelProperty(
    string name,
    string type,
    string accessModifier,
    IReadOnlyCollection<AttributeData> attributes)
{
    public string Name { get; } = name;
    public string Type { get; } = type;
    public string AccessModifier { get; } = accessModifier;
    public IReadOnlyCollection<AttributeData> Attributes { get; } = attributes;
}
