using KurzSharp;

namespace TestApi.Models;

// NOTE: Can be accessed at `/product`
[RestApi]
public class Product : IBeforeReadHook, IBeforeCreateHook, IBeforeDeleteHook, IBeforeUpdateHook
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public void OnBeforeCreate()
    {
        Id = Guid.NewGuid();
        Console.WriteLine("Product created");
        Console.WriteLine(Id.ToString());
    }

    public void OnBeforeRead()
    {
        Password = string.Empty;
        Console.WriteLine($"Reading {nameof(Product)} with Id: {Id}");
    }

    public void OnBeforeDelete()
    {
        Console.WriteLine($"Deleting {nameof(Product)} with Id: {Id}");
    }

    public void OnBeforeUpdate()
    {
        if (!Name.Contains("Mr/Ms"))
        {
            Name = $"Mr/Ms {Name}";
        }
    }
}