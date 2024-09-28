# KurzSharp

[![NuGet Publish](https://github.com/ahmad2smile/KurzSharp/actions/workflows/publish.yml/badge.svg??event=push)](https://github.com/ahmad2smile/KurzSharp/actions/workflows/publish.yml)
[![Nuget](https://img.shields.io/nuget/v/KurzSharp)](https://www.nuget.org/packages/KurzSharp/)
[![Nuget](https://img.shields.io/nuget/dt/KurzSharp)](https://www.nuget.org/stats/packages/KurzSharp?groupby=Version)
[![GitHub](https://img.shields.io/github/license/ahmad2smile/KurzSharp)](LICENSE)

Easily scaffold non-production APIs in any or all of following Architectures:

1. GraphQl ([HotChocolate](https://github.com/ChilliCream/graphql-platform))
2. Grpc (Code First [protobuf-net.Grpc](https://github.com/protobuf-net/protobuf-net.Grpc))
3. REST ([ASP.NET Core Controllers](https://learn.microsoft.com/en-us/aspnet/core/web-api))

Minimum required dotnet version: [.NET8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## Usage

1. Create any model with a `partial class` and add relevant Attribute for ex: `RestApi`

```csharp
[GrpcApi] // Whicherver is desired
[RestApi]
[GraphQlApi]
public partial class Product
{
    public Guid Id { get; set; }
}
```

2. Configure `KurzSharp` by default Data is stored in Memory with Entity Framework `UseInMemoryDatabase`

```csharp
services.AddKurzSharp();
```

For Database add relavent Db package for Entity Framework and configure `KurzSharp` similar way to Entity Framework
config. (ex: PostgresDb):

```csharp
services.AddKurzSharp(o => o.UseNpgsql(configuration.GetConnectionString("ProductsDb")));
```

Map Routes and Services:

```csharp
app.MapKurzSharpServices();
```

🎉 You API is ready, Run project and open Swagger Docs.

For GraphQl API open [Bana Cake Pop](https://chillicream.com/docs/bananacakepop)
at [http://localhost:5114/graphql](http://localhost:5114/graphql)

![image](https://github.com/user-attachments/assets/54db1ac3-6d08-4945-8fc1-3db448080089)


For REST or Grpc API open [Swagger](https://swagger.io/)
at [http://localhost:5114/swagger](http://localhost:5114/swagger)

![image](https://github.com/user-attachments/assets/ba23eaf3-dbbb-4775-a290-94871e7e6841)


For more information please check `examples/TestApi`

### Features:

- Hook into process to control how/what information on Entity is modified/observed with following hooks. Hook
  automatically attached the Model, just `override` the required ones.
    - `TDto OnBeforeCreate(TDto dto)`
    - `IEnumerable<TDto> OnBeforeCreate(IEnumerable<TDto> dtos)`
    - `IQueryable<TDto> OnBeforeRead(IQueryable<TDto> dtos)`
    - `TDto OnBeforeRead(TDto dto)`
    - `TDto OnBeforeUpdate(TDto dto)`
    - `Enumerable<TDto> OnBeforeUpdate(IEnumerable<TDto> dto)`
    - `TDto OnBeforeDelete(TDto dto)`
    - `IEnumerable<TDto> OnBeforeDelete(IEnumerable<TDto> dto)`


Ex:

```csharp

[RestApi]
public partial class Product
{
    
    private readonly ILogger<Product> _logger;

    public Product(ILogger<Product> logger)
    {
        _logger = logger;
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public override ProductDto OnBeforeCreate(ProductDto dto)
    {
        _logger.LogInformation("DI is working...");

        return dto;
    }
}
```

## How it works:

When a `KurzSharp` attributes like `RestApi`, `GraphQlApi` or `GrpcApi` is added on some Model class, it creates a new
`Dto` entity which has the same properties as the Model. The `Model` is used to store data through
`Entity Framework Core` and `Dto` is sent over the wire through API. It also takes attributes from the properties and
puts them on the `Dto`'s properties for example: `JsonIgnoreAttribute`.
