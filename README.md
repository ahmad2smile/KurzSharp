# Kurz Sharp

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

```csharp
services.AddKurzSharp(o => o.UseNpgsql(configuration.GetConnectionString("ProductsDb")));
```

🎉 You API is ready, Run project and open Swagger Docs.

For more information please check `test/TestApi`