namespace KurzSharp.Templates.GraphQlApi;

public static class GraphQlExceptions
{
    public class AddEntityException(string message) : Exception(message);

    public class AddEntitiesException(string message) : Exception(message);

    public class DeleteEntityException(string message) : Exception(message);

    public class DeleteEntitiesException(string message) : Exception(message);

    public class UpdateEntityException(string message) : Exception(message);

    public class UpdateEntitiesException(string message) : Exception(message);
}
