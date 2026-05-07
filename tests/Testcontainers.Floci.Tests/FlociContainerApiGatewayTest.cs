using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.APIGateway;
using Amazon.APIGateway.Model;
using Amazon.Runtime;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerApiGatewayTest(FlociContainerFixture fixture)
{
    private AmazonAPIGatewayClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonAPIGatewayConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task ApiGateway_CreateAndListRestApis_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"api-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        var created = await client.CreateRestApiAsync(new CreateRestApiRequest { Name = name }, ct);
        var response = await client.GetRestApisAsync(new GetRestApisRequest(), ct);

        Assert.False(string.IsNullOrWhiteSpace(created.Id));
        Assert.Contains(response.Items, a => a.Name == name);
    }

    [Fact]
    public async Task ApiGateway_CreateResource_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"api-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        var api = await client.CreateRestApiAsync(new CreateRestApiRequest { Name = name }, ct);
        var resources = await client.GetResourcesAsync(new GetResourcesRequest { RestApiId = api.Id }, ct);
        var rootId = resources.Items.First(r => r.Path == "/").Id;
        var created = await client.CreateResourceAsync(new CreateResourceRequest
        {
            RestApiId = api.Id,
            ParentId = rootId,
            PathPart = "test",
        }, ct);

        Assert.False(string.IsNullOrWhiteSpace(created.Id));
    }
}
