using System.Text;
using KurzSharp.Templates.Models;

namespace KurzSharp.Utils;

public static class TemplatesUtils
{
    public const string PlaceholderTypeName = nameof(PlaceholderModel);
    public const string PlaceholderDtoTypeName = nameof(PlaceholderModelDto);

    private static readonly string PlaceholderTypeCamelCase =
        PlaceholderTypeName.Substring(0, 1).ToLowerInvariant() + PlaceholderTypeName.Substring(1);

    private const string ProjectNamespace = "KurzSharp";
    private const string TemplateNamespace = "Templates";

    /// <summary>
    /// Gets source code from embedded resource file under Templates
    /// </summary>
    /// <param name="fileName">Name of the embedded resource file without extension ex: MyTemplate</param>
    /// <returns>Source code as string</returns>
    public static string GetTemplateFileContent(string fileName) =>
        GetFileContent($"{ProjectNamespace}.{TemplateNamespace}.{fileName}.cs");

    /// <summary>
    /// Gets source code from embedded resource file under Templates
    /// </summary>
    /// <param name="fileTemplateDirectory">File directory under Templates directory</param>
    /// <param name="fileName">Name of the embedded resource file without extension ex: MyTemplate</param>
    /// <param name="extension">File Extension ex: cs, js, proto</param>
    /// <returns>Source code as string</returns>
    public static string
        GetTemplateFileContent(string fileTemplateDirectory, string fileName, string extension = "cs") =>
        // NOTE:
        // Assumption 1: Templates are in valid directory based namespace
        // Assumption 2: Templates filenames are same as Type name
        GetFileContent($"{ProjectNamespace}.{TemplateNamespace}.{fileTemplateDirectory}.{fileName}.{extension}");

    /// <summary>
    /// Gets source code from embedded resource file
    /// </summary>
    /// <param name="fullyQualifiedFileName">Name of the embedded resource file with namespace ex: MyProject.Templates.MyClass.cs</param>
    /// <returns>Source code as string</returns>
    /// <exception cref="InvalidOperationException">Throws when template file not found or empty</exception>
    public static string GetFileContent(string fullyQualifiedFileName)
    {
        using var stream = typeof(TemplatesUtils).Assembly.GetManifestResourceStream(fullyQualifiedFileName);

        if (stream is null)
        {
            throw new InvalidOperationException($"Template file {fullyQualifiedFileName} not found");
        }

        using var reader = new StreamReader(stream, Encoding.UTF8);

        var fileContent = reader.ReadToEnd();

        if (string.IsNullOrWhiteSpace(fileContent))
        {
            throw new InvalidOperationException($"Template file {fullyQualifiedFileName} is empty");
        }

        return fileContent;
    }

    /// <summary>
    /// Replace `PlaceholderModel` references in Code with actual type
    /// </summary>
    /// <param name="source"></param>
    /// <param name="typeName">Type to be used to replace `PlaceholderModel`</param>
    /// <returns></returns>
    public static string ReplacePlaceholderType(this string source, string typeName)
    {
        // NOTE: Doesn't work .netstandard2.0
        // ReSharper disable once ReplaceSubstringWithRangeIndexer
        var typeCamelCase = typeName.Substring(0, 1).ToLowerInvariant() + typeName.Substring(1);

        return source.Replace(PlaceholderTypeName, typeName)
            .Replace(PlaceholderTypeCamelCase, typeCamelCase);
    }

    /// <summary>
    /// Removes `.Templates` from Namespaces
    /// </summary>
    /// <param name="source"></param>
    /// <param name="typeNamespace"></param>
    /// <returns></returns>
    public static string FixupNamespaces(this string source, string typeNamespace)
    {
        var modelsNamespace = typeNamespace.Contains("Models") ? typeNamespace : $"{typeNamespace}.Models";

        return source.Replace($".{TemplateNamespace}", string.Empty)
            .Replace($"{ProjectNamespace}.Models", modelsNamespace);
    }

    /// <summary>
    /// Add Using Statements for given namespace if non-existent
    /// </summary>
    /// <param name="source"></param>
    /// <param name="typeNamespace">Namespace to add using statements for</param>
    /// <returns></returns>
    public static string AddUsing(this string source, string typeNamespace)
    {
        return source.AddUsing(new[] { typeNamespace });
    }

    /// <summary>
    /// Add Using Statements for given namespaces if non-existent
    /// </summary>
    /// <param name="source"></param>
    /// <param name="typeNamespaces">Namespaces to add using statements for</param>
    /// <returns></returns>
    public static string AddUsing(this string source, IEnumerable<string> typeNamespaces)
    {
        foreach (var ns in typeNamespaces)
        {
            var nsUsing = $"using {ns};";

            if (source.Contains(nsUsing))
            {
                continue;
            }

            source = $"{nsUsing}\n{source}";
        }

        return source;
    }
}
