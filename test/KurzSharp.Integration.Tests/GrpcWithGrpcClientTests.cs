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
    public async Task Operations(List<ProductDto> data, List<ProductDto> updatedData)
    {
        var dataIds = data.Select(static i => i.Id).ToList();

        while (data.Count < updatedData.Count)
        {
            data.Add(updatedData[data.Count - 1]);
        }

        var client = factory.Services.GetRequiredService<IProductGrpcService>();

        // INSERT
        await Task.WhenAll(data.Select(dto => client.AddProduct(dto, CancellationToken.None)));

        // READ
        var afterAddDtos = await GetAll(dataIds);

        afterAddDtos.Select(i => i.Id).ToList().Should().Contain(dataIds);

        // UPDATE
        await Task.WhenAll(afterAddDtos.Select((dto, i) =>
        {
            var updateDto = updatedData[i];

            dto.Name = updateDto.Name;
            dto.Password = updateDto.Password;

            return client.UpdateProduct(dto, CancellationToken.None);
        }));

        var afterUpdateDtos = await GetAll(dataIds);

        afterUpdateDtos.Select(i => i.Id).ToList().Should().Contain(dataIds);

        // DELETE
        await client.DeleteProducts(data, CancellationToken.None);

        var afterDeletedRes = await GetAll(dataIds);

        afterDeletedRes.Select(d => d.Id).Should().NotContain(afterUpdateDtos.Select(d => d.Id));
    }

    private async Task<IList<ProductDto>> GetAll(List<Guid> relatedIds)
    {
        var client = factory.Services.GetRequiredService<IProductGrpcService>();

        var result = await client.GetProducts(CancellationToken.None);

        return result?.Where(r => relatedIds.Contains(r.Id)).ToList() ?? [];
    }
}
