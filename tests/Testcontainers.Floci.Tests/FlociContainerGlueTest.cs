using System;
using System.Threading.Tasks;
using Amazon.Glue;
using Amazon.Glue.Model;
using Amazon.Runtime;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerGlueTest(FlociContainerFixture fixture)
{
    private AmazonGlueClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonGlueConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task Glue_CreateAndListDatabase_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"db{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.CreateDatabaseAsync(new CreateDatabaseRequest { DatabaseInput = new DatabaseInput { Name = name } }, ct);
        var response = await client.GetDatabasesAsync(new GetDatabasesRequest(), ct);

        Assert.Contains(response.DatabaseList, d => d.Name == name);
    }

    [Fact]
    public async Task Glue_GetDatabase_ReturnsExpectedName()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"db{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.CreateDatabaseAsync(new CreateDatabaseRequest { DatabaseInput = new DatabaseInput { Name = name } }, ct);
        var response = await client.GetDatabaseAsync(new GetDatabaseRequest { Name = name }, ct);

        Assert.Equal(name, response.Database.Name);
    }
}
