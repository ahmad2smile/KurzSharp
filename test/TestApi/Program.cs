using KurzSharp;
using Microsoft.OpenApi.Models;
using TestApi;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddKurzSharp();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
// app.MapGrpcService<ProductService>();

app.MapKurzSharpServices();

app.Run();

public partial class Program
{
}
