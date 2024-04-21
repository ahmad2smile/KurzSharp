# Kurz Sharp

[![NuGet Publish](https://github.com/ahmad2smile/KurzSharp/actions/workflows/publish.yml/badge.svg?branch=master&event=release)](https://github.com/ahmad2smile/KurzSharp/actions/workflows/publish.yml)
[![Nuget](https://img.shields.io/nuget/v/KurzSharp)](https://www.nuget.org/packages/KurzSharp/)
[![Nuget](https://img.shields.io/nuget/dt/KurzSharp)](https://www.nuget.org/stats/packages/KurzSharp?groupby=Version)
[![GitHub](https://img.shields.io/github/license/ahmad2smile/KurzSharp)](LICENSE)

Easily scaffold API for non-production scenarios.

## Usage

1. Create any model with a `partial class` and add relevant Attribute for ex: `RestApi`

```csharp
[RestApi]
public partial class Product
{
    public Guid Id { get; set; }
}
```

2. Configure `KurzSharp` by default Data is stored in Memory with Entity Framework `UseInMemoryDatabase`

```csharp
services.AddKurzSharp();
```

For Database add relavent Db package for Entity Framework and configure `KurzSharp` similar way to Entity Framework config. (ex: PostgresDb):

```csharp
services.AddKurzSharp(o => o.UseNpgsql(configuration.GetConnectionString("ProductsDb")));
```

🎉 You API is ready, Run project and open Swagger Docs.

For more information please check `test/TestApi`

### Features:

- Hook into process to control how/what information on Entity is modified/observed with following hooks. Hook
  automatically attached the Model, just `override` the required ones.
    - `OnBeforeCreate`
    - `OnBeforeAllRead`
    - `OnBeforeRead`
    - `OnBeforeUpdate`
    - `OnBeforeDelete`

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
