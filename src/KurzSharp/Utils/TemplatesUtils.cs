using System.Text;
using KurzSharp.Templates.Models;
using Microsoft.CodeAnalysis;

namespace KurzSharp.Utils;

public static class TemplatesUtils
{
    private const string PlaceholderTypeName = nameof(PlaceholderModel);

    private static readonly string PlaceholderTypeCamelCase =
        PlaceholderTypeName.Substring(0, 1).ToLowerInvariant() + PlaceholderTypeName.Substring(1);

    private const string ProjectNamespace = "KurzSharp";
    private const string TemplateNamespace = "Templates";

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

        return fileContent;
    }

    public static string FixReferences(this string source, ModelSourceInfo sourceInfo)
    {
        var typeName = sourceInfo.TypeName;
        var typeCamelCase = typeName.Substring(0, 1).ToLowerInvariant() + typeName.Substring(1);

        return source
            // Fixup Namespaces
            .Replace($"{ProjectNamespace}.{TemplateNamespace}.Models", sourceInfo.TypeNamespace)
            .Replace(".Templates.", ".")
            .Replace($"{ProjectNamespace}.{TemplateNamespace}", ProjectNamespace)
            // Fixup Placeholder types
            .Replace(PlaceholderTypeName, typeName)
            .Replace(PlaceholderTypeCamelCase, typeCamelCase);
    }

    /// <summary>
    /// Get <see cref="AttributeData"/> of a give Property and converts it into Attribute Syntax: [Attribute(Arg = Value)]
    /// </summary>
    /// <param name="propertySymbol"></param>
    /// <returns></returns>
    public static string GetPropAttrSrc(IPropertySymbol propertySymbol) =>
        propertySymbol.GetAttributes().Aggregate("\n", (syntax, attributeData) =>
        {
            var namedArgs = attributeData.NamedArguments.Select((namedArg) => $"{namedArg.Key}={namedArg.Value.Value}");
            var constructorArgs = attributeData.ConstructorArguments.Select(b => b.Value?.ToString());

            var args = string.Join(", ", namedArgs.Concat(constructorArgs));

            return syntax + $"[{attributeData.AttributeClass}({args})]";
        });
}
