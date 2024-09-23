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
        await Task.WhenAll(data.Select(dto => client.PostAsync(BaseUrl,
            new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"))));

        // READ
        var afterAddDtos = await GetAll(dataIds);

        afterAddDtos.Select(i => i.Id).ToList().Should().Contain(dataIds);

        // UPDATE
        await Task.WhenAll(afterAddDtos.Select((dto, i) =>
        {
            var updateDto = updatedData[i];

            dto.Name = updateDto.Name;
            dto.Password = updateDto.Password;

            return client.PutAsync(BaseUrl,
                new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
        }));


        var updatedResults = await GetAll(dataIds);

        updatedResults.Select(i => i.Id).ToList().Should().Contain(dataIds);

        // DELETE
        await Task.WhenAll(data.Select(dto =>
        {
            var request = new HttpRequestMessage
            {
                Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(client.BaseAddress!, BaseUrl)
            };

            return client.SendAsync(request);
        }));

        var afterDeletedRes = await GetAll(dataIds);

        afterDeletedRes.Should().NotContain(updatedResults);
    }

    private async Task<IList<ProductDto>> GetAll(List<Guid> relatedIds)
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(BaseUrl);

        var result = await response.Content.ReadFromJsonAsync<IList<ProductDto>>();

        return result?.Where(r => relatedIds.Contains(r.Id)).ToList() ?? [];
    }
}
