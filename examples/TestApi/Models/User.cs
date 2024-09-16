using KurzSharp;

namespace TestApi.Models;

[GrpcApi]
[GraphQlApi]
public partial class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}
