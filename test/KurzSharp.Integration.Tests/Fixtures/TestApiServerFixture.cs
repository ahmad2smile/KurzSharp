using KurzSharp.GrpcApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.ClientFactory;

namespace KurzSharp.Integration.Tests.Fixtures;

public class TestApiServerFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddCodeFirstGrpcClient<IProductGrpcService>(o => { o.Address = Server.BaseAddress; })
                .ConfigureChannel((_, o) => { o.HttpHandler = Server.CreateHandler(); });
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        client.BaseAddress = new Uri("http://localhost");
    }
}
