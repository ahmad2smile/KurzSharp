using KurzSharp;

namespace TestApi.Models;

[RestApi]
public partial class Book
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }
}
