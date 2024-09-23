using System.Text;
using System.Text.Json;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TestApi.Models;

namespace KurzSharp.Integration.Tests;

public class GraphQlClientTests
{
    private readonly WebApplicationFactory<Program> _factory = new();
    private const string BaseUrl = "/graphql";

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
            var payload = new
            {
                query = $$"""
                          mutation Mutation {
                            addProduct(input: { id: "{{dto.Id}}",  name: "{{dto.Name}}", password: "{{dto.Password}}" }) {
                              id
                              name
                            }
                          }
                          """
            };
            await client.PostAsync(BaseUrl,
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
        }

        // READ
        var results = await GetAll(dataIds);

        results.Select(i => i.Id).ToList().Should().Contain(dataIds);

        // UPDATE
        for (var i = 0; i < data.Count; i++)
        {
            var dto = results[i];
            dto.Name = updatedData[i].Name;
            dto.Password = updatedData[i].Password;

            var payload = new
            {
                query = $$"""
                          mutation Mutation {
                            updateProduct(input: { id: "{{dto.Id}}",  name: "{{dto.Name}}", password: "{{dto.Password}}" }) {
                              id
                              name
                            }
                          }
                          """
            };

            await client.PostAsync(BaseUrl,
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
        }

        var updatedResults = await GetAll(dataIds);

        updatedResults.Select(i => i.Id).Should().Contain(dataIds);

        // DELETE
        foreach (var dto in updatedResults)
        {
            var payload = new
            {
                query = $$"""
                          mutation Mutation {
                            deleteProduct(input: { id: "{{dto.Id}}",  name: "{{dto.Name}}", password: "{{dto.Password}}" }) {
                              id
                              name
                            }
                          }
                          """
            };

            await client.PostAsync(BaseUrl,
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
        }

        var afterDeletedRes = await GetAll(dataIds);

        afterDeletedRes.Should().NotContain(updatedResults);
    }

    private async Task<IList<ProductDto>> GetAll(List<Guid> relatedIds)
    {
        var client = _factory.CreateClient();

        var payload = new
        {
            query = """
                    query Query {
                      products {
                        nodes {
                          id
                          name
                          password
                        }
                      }
                    }
                    """
        };
        var response = await client.PostAsync(BaseUrl,
            new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

        var nodes = await ParseProductDtos(response, "products");

        return nodes.Where(r => relatedIds.Contains(r.Id)).ToList();
    }

    private static async Task<List<ProductDto>> ParseProductDtos(HttpResponseMessage response, string dataKey)
    {
        var result = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(result);
        var root = doc.RootElement;

        // Access properties dynamically
        var nodes = root.GetProperty("data").GetProperty(dataKey).GetProperty("nodes").EnumerateArray().Select(n =>
            new ProductDto
            {
                Id = Guid.Parse(n.GetProperty("id").GetString()!),
                Name = n.GetProperty("name").GetString(),
                Password = n.GetProperty("password").GetString(),
            });
        return nodes.ToList();
    }
}
