using KurzSharp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKurzSharp();

var app = builder.Build();

app.MapKurzSharpServices();

app.Run();

// NOTE: This is only required for Testing
public partial class Program
{
}
