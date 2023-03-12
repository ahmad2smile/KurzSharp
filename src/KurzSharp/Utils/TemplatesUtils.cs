using System.Text;

namespace KurzSharp.Utils;

public static class TemplatesUtils
{
    public static string GetTemplateFileContent(string fileNamespace, string fileName)
    {
        // NOTE:
        // Assumption 1: Templates are in valid directory based namespace
        // Assumption 2: Templates filenames are same as Type name
        var resource = $"{fileNamespace}.{fileName}.cs";

        using var stream = typeof(TemplatesUtils).Assembly.GetManifestResourceStream(resource);

        if (stream is null)
        {
            throw new InvalidOperationException($"Template file {resource} not found");
        }

        using var reader = new StreamReader(stream, Encoding.UTF8);

        var fileContent = reader.ReadToEnd();

        if (string.IsNullOrWhiteSpace(fileContent))
        {
            throw new InvalidOperationException($"Template file {resource} is empty");
        }

        return fileContent;
    }

    private const string PlaceholderTypeName = nameof(PlaceholderModel);

    private static readonly string PlaceholderTypeCamelCase =
        PlaceholderTypeName.Substring(0, 1).ToLowerInvariant() + PlaceholderTypeName.Substring(1);

    private const string TemplateNamespace = "Templates";

    /// <summary>
    /// Replace `PlaceholderModel` references in Code with actual type
    /// </summary>
    /// <param name="source"></param>
    /// <param name="typeName">Type to be used to replace `PlaceholderModel`</param>
    /// <returns></returns>
    public static string ReplacePlaceholderType(this string source, string typeName)
    {
        var typeCamelCase = typeName.Substring(0, 1).ToLowerInvariant() + typeName.Substring(1);

        return source.Replace(PlaceholderTypeName, typeName)
            .Replace(PlaceholderTypeCamelCase, typeCamelCase);
    }

    /// <summary>
    /// Removes `.Templates` from Namespaces
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string FixupNamespaces(this string source)
    {
        return source.Replace($".{TemplateNamespace}", "");
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