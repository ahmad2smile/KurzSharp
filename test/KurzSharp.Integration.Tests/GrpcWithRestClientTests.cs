using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AutoFixture.Xunit2;
using FluentAssertions;
using KurzSharp.Integration.Tests.Fixtures;
using TestApi.Models;

namespace KurzSharp.Integration.Tests;

public class GrpcWithRestClientTests(TestApiServerFixture factory) : IClassFixture<TestApiServerFixture>
{
    private const string BaseUrl = $"/{nameof(Product)}Grpc";

    [Theory, AutoData]
    public async Task Operations(List<ProductDto> data, List<ProductDto> updatedData)
    {
        var dataIds = data.Select(i => i.Id).ToList();

        while (data.Count < updatedData.Count)
        {
            data.Add(updatedData[data.Count - 1]);
        }

        var client = factory.CreateClient();

        // INSERT
        foreach (var dto in data)
        {
            await client.PostAsync(BaseUrl,
                new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
        }

        // READ
        var results = await GetAll();

        results.Select(i => i.Id).ToList().Should().Contain(dataIds);

        // UPDATE
        for (var i = 0; i < data.Count; i++)
        {
            var dto = results[i];
            dto.Name = updatedData[i].Name;
            dto.Password = updatedData[i].Password;

            await client.PutAsync(BaseUrl,
                new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
        }

        var updatedResults = await GetAll();

        updatedResults.Select(i => i.Id).ToList().Should().Contain(dataIds);

        // DELETE
        foreach (var dto in data)
        {
            var request = new HttpRequestMessage
            {
                Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(client.BaseAddress!, BaseUrl)
            };

            await client.SendAsync(request);
        }

        var afterDeletedRes = await GetAll();

        afterDeletedRes.Should().NotContain(data);
    }

    private async Task<IList<ProductDto>> GetAll()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(BaseUrl);

        var result = await response.Content.ReadFromJsonAsync<IList<ProductDto>>();

        return result ?? ArraySegment<ProductDto>.Empty;
    }
}
