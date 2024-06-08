using AutoFixture.Xunit2;
using FluentAssertions;
using KurzSharp.GrpcApi;
using KurzSharp.Integration.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using TestApi.Models;

namespace KurzSharp.Integration.Tests;

public class GrpcWithGrpcClientTests(TestApiServerFixture factory) : IClassFixture<TestApiServerFixture>
{
    [Theory, AutoData]
    public async Task Operations(List<ProductDto> d, List<ProductDto> updatedData)
    {
        var data = d.Select(static p =>
        {
            p.Id = Guid.NewGuid();
            return p;
        }).ToList();

        var dataIds = data.Select(static i => i.Id).ToList();

        while (data.Count < updatedData.Count)
        {
            data.Add(updatedData[data.Count - 1]);
        }

        var client = factory.Services.GetRequiredService<IProductGrpcService>();

        // INSERT
        foreach (var dto in data)
        {
            await client.AddProduct(dto, CancellationToken.None);
        }

        // READ
        var serviceResult = await client.GetProducts(CancellationToken.None);
        var results = serviceResult.ToList();

        results.Select(i => i.Id).ToList().Should().Contain(dataIds);

        // UPDATE
        for (var i = 0; i < data.Count; i++)
        {
            var dto = results[i];
            dto.Name = updatedData[i].Name;
            dto.Password = updatedData[i].Password;

            await client.UpdateProduct(dto, CancellationToken.None);
        }

        serviceResult = await client.GetProducts(CancellationToken.None);
        var updatedResults = serviceResult.ToList();

        updatedResults.Select(i => i.Id).ToList().Should().Contain(dataIds);

        // DELETE
        foreach (var dto in data)
        {
            await client.DeleteProduct(dto, CancellationToken.None);
        }

        serviceResult = await client.GetProducts(CancellationToken.None);
        var afterDeletedRes = serviceResult.ToList();

        afterDeletedRes.Should().NotContain(data);
    }
}
