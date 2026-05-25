using System.Text;
using KurzSharp.Templates.Models;
using Microsoft.CodeAnalysis;

namespace KurzSharp.Utils;

public static class TemplatesUtils
{
    private const string ProjectNamespace = "KurzSharp";
    private const string TemplateNamespace = "Templates";

    public static string ToCamelCase(this string value) =>
        // NOTE: Disable for netstandard2.0
        // ReSharper disable file ReplaceSubstringWithRangeIndexer
#pragma warning disable CA1845
        value.Substring(0, 1).ToLowerInvariant() + value.Substring(1);
#pragma warning restore CA1845

    /// <summary>
    /// Gets source code from embedded resource file under Templates
    /// </summary>
    /// <param name="fileName">Name of the embedded resource file without extension ex: MyTemplate</param>
    /// <param name="fileTemplateDirectory">File directory under Templates directory</param>
    /// <param name="extension">File Extension ex: cs, js, proto</param>
    /// <returns>Source code as string</returns>
    public static string GetFileContent(string fileName, string fileTemplateDirectory = "", string extension = "cs")
    {
        // NOTE:
        // Assumption 1: Templates are in valid directory based namespace
        // Assumption 2: Templates filenames are same as Type name
        var fullyQualifiedFileName =
            $"{ProjectNamespace}.{TemplateNamespace}.{fileTemplateDirectory}.{fileName}.{extension}".Replace("..", ".");

        using var stream = typeof(TemplatesUtils).Assembly.GetManifestResourceStream(fullyQualifiedFileName);

        if (stream is null)
        {
            throw new InvalidOperationException($"Template file {fullyQualifiedFileName} not found, stream null");
        }

        var buffer = new byte[stream.Length];
        _ = stream.Read(buffer, 0, buffer.Length);

        var fileContent = Encoding.Default.GetString(buffer);

        if (string.IsNullOrWhiteSpace(fileContent))
        {
            throw new InvalidOperationException($"Template file {fullyQualifiedFileName} is empty");
        }

        // Normalize line endings so multi-line string replacements (e.g. removing the generated
        // constructor) match regardless of how the generator's sources were checked out.
        return fileContent.Replace("\r\n", "\n");
    }

    public static string FixReferences(this string source, ModelSourceInfo sourceInfo)
    {
        var typeName = sourceInfo.TypeName;
        var typeCamelCase = typeName.Substring(0, 1).ToLowerInvariant() + typeName.Substring(1);
        var key = sourceInfo.KeyProperty;

        if (key != null)
        {
            if (!key.Type.Contains(nameof(Guid)))
            {
                // Retype the get-by-id parameter and the gRPC id wrapper to the model's key type.
                // Target the specific declarations rather than a blind `Guid id`/`Guid Id` substring
                // replace, which would corrupt unrelated Guid-typed properties (e.g. a property named
                // `Id` or `identifier`).
                source = source.Replace("GetPlaceholderModel(Guid id", $"GetPlaceholderModel({key.Type} id");
                source = source.Replace("public Guid Id { get; set; }", $"public {key.Type} Id {{ get; set; }}");
            }

            if (key.Name != "Id")
            {
                // The generated lookup hardcodes `m.Id`; point it at the actual key property.
                source = source.Replace("m.Id == id", $"m.{key.Name} == id");
            }
        }

        return source
            // Fixup Namespaces
            .Replace($"{ProjectNamespace}.{TemplateNamespace}.Models", sourceInfo.TypeNamespace)
            .Replace(".Templates.", ".")
            .Replace($"{ProjectNamespace}.{TemplateNamespace}", ProjectNamespace)
            // Fixup Placeholder types
            .Replace(nameof(PlaceholderModel), typeName)
            .Replace(nameof(PlaceholderModel).ToCamelCase(), typeCamelCase);
    }

    /// <summary>
    /// Get <see cref="AttributeData"/> of a give Property and converts it into Attribute Syntax: [Attribute(Arg = Value)]
    /// </summary>
    /// <param name="modelProperty"></param>
    /// <returns></returns>
    public static string GetPropAttrSrc(ModelProperty modelProperty) =>
        modelProperty.Attributes.Aggregate("\n", (syntax, attributeData) =>
        {
            var namedArgs = attributeData.NamedArguments.Select((namedArg) => $"{namedArg.Key}={namedArg.Value.Value}");
            var constructorArgs = attributeData.ConstructorArguments.Select(b => b.Value?.ToString());

            var args = string.Join(", ", namedArgs.Concat(constructorArgs));

            return syntax + $"[{attributeData.AttributeClass}({args})]";
        });
}
