using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TestApi.Models;

namespace KurzSharp.Integration.Tests;

public class RestClientTests
{
    private readonly WebApplicationFactory<Program> _factory = new();
    private const string BaseUrl = $"/{nameof(Product)}Rest";

    [Theory, AutoData]
    public async Task Operations(List<ProductDto> data, List<ProductDto> updatedData)
    {
        var dataIds = data.Select(i => i.Id).ToList();

        while (data.Count < updatedData.Count)
        {
            data.Add(updatedData[data.Count - 1]);
        }

        var client = _factory.CreateClient();

        // INSERT
        foreach (var dto in data)
        {
            await client.PostAsync(BaseUrl,
                new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
        }

        // READ
        var afterAddDtos = await GetAll(dataIds);

        afterAddDtos.Select(i => i.Id).ToList().Should().Contain(dataIds);

        // UPDATE
        for (var i = 0; i < data.Count; i++)
        {
            var dto = afterAddDtos[i];
            dto.Name = updatedData[i].Name;
            dto.Password = updatedData[i].Password;

            await client.PutAsync(BaseUrl,
                new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
        }

        var afterUpdateDtos = await GetAll(dataIds);

        afterUpdateDtos.Select(i => i.Id).Should().Contain(dataIds);

        // DELETE
        foreach (var dto in afterUpdateDtos)
        {
            var request = new HttpRequestMessage
            {
                Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(client.BaseAddress!, BaseUrl)
            };

            await client.SendAsync(request);
        }

        var afterDeletedRes = await GetAll(dataIds);

        afterDeletedRes.Should().NotContain(afterUpdateDtos);
    }

    private async Task<IList<ProductDto>> GetAll(List<Guid> relatedIds)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(BaseUrl);

        var result = await response.Content.ReadFromJsonAsync<IList<ProductDto>>();

        return result?.Where(r => relatedIds.Contains(r.Id)).ToList() ?? [];
    }
}
