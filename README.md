# Kurz Sharp

[![NuGet Publish](https://github.com/ahmad2smile/KurzSharp/actions/workflows/publish.yml/badge.svg?branch=master&event=release)](https://github.com/ahmad2smile/KurzSharp/actions/workflows/publish.yml)
[![Nuget](https://img.shields.io/nuget/v/KurzSharp)](https://www.nuget.org/packages/KurzSharp/)
[![Nuget](https://img.shields.io/nuget/dt/KurzSharp)](https://www.nuget.org/stats/packages/KurzSharp?groupby=Version)
[![GitHub](https://img.shields.io/github/license/ahmad2smile/KurzSharp)](LICENSE)

Easily scaffold API for non-production scenarios.

## Usage

1. Create any model and add relevant Attribute for ex: `RestApi`

```csharp
[RestApi]
public class Product
{
    public Guid Id { get; set; }
}
```

2. Configure `KurzSharp`

For In-Memory:

```csharp
services.AddKurzSharp(o => o.UseInMemoryDatabase("ProductsDb"));
```

For Database (PostgresDb):

```csharp
services.AddKurzSharp(o => o.UseNpgsql(configuration.GetConnectionString("ProductsDb")));
```

🎉 You API is ready, Run project and open Swagger Docs.

For more information please check `test/TestApi`