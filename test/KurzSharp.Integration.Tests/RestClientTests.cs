using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc.Testing;
using TestApi.Models;

namespace KurzSharp.Integration.Tests;

public class RestClientTests
{
    private readonly WebApplicationFactory<Program> _factory = new();

    [Theory, AutoData]
    public async Task Operations(List<ProductDto> data, List<ProductDto> updatedData)
    {
        while (data.Count < updatedData.Count)
        {
            data.Add(updatedData[data.Count - 1]);
        }

        var client = _factory.CreateClient();

        // INSERT
        foreach (var dto in data)
        {
            await client.PostAsync($"/{nameof(Product)}",
                new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
        }

        // READ
        var results = await GetAll();

        Assert.Equal(data.Count, results!.Count);

        for (var i = 0; i < data.Count; i++)
        {
            Assert.Equal(data[i].Id, results[i].Id);
            Assert.Equal(data[i].Name, results[i].Name);
            Assert.Equal(data[i].Password, results[i].Password);
        }

        // UPDATE
        for (var i = 0; i < data.Count; i++)
        {
            var dto = results[i];
            dto.Name = updatedData[i].Name;
            dto.Password = updatedData[i].Password;

            await client.PutAsync($"/{nameof(Product)}",
                new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));
        }

        var updatedResults = await GetAll();

        Assert.Equal(data.Count, updatedResults!.Count);

        for (var i = 0; i < data.Count; i++)
        {
            Assert.Equal(data[i].Id, updatedResults[i].Id);
            Assert.Equal(updatedData[i].Name, updatedResults[i].Name);
            Assert.Equal(updatedData[i].Password, updatedResults[i].Password);
        }

        // DELETE
        foreach (var dto in updatedResults)
        {
            var request = new HttpRequestMessage
            {
                Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(client.BaseAddress!, $"{nameof(Product)}")
            };

            await client.SendAsync(request);
        }

        var afterDeletedRes = await GetAll();

        Assert.Equal(0, afterDeletedRes!.Count);
    }

    private async Task<IList<ProductDto>?> GetAll()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/{nameof(Product)}");

        return await response.Content.ReadFromJsonAsync<IList<ProductDto>>();
    }
}
