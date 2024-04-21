using KurzSharp;

namespace TestApi.Models;

// NOTE: Can be accessed at `/product`
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

        Console.WriteLine("Product created");

        return dto;
    }

    public override IEnumerable<ProductDto> OnBeforeAllRead(IEnumerable<ProductDto> dtos)
    {
        _logger.LogInformation("Before we read all... we must log here");

        return base.OnBeforeAllRead(dtos);
    }

    public override ProductDto OnBeforeRead(ProductDto dto)
    {
        Password = string.Empty;
        Console.WriteLine($"Reading {nameof(Product)} with Id: {Id}");

        return base.OnBeforeRead(dto);
    }

    public override ProductDto OnBeforeDelete(ProductDto dto)
    {
        Console.WriteLine($"Deleting {nameof(Product)} with Id: {Id}");

        return dto;
    }

    public override ProductDto OnBeforeUpdate(ProductDto dto)
    {
        if (!Name.Contains("Mr/Ms"))
        {
            Name = $"Mr/Ms {Name}";
        }

        return dto;
    }
}
