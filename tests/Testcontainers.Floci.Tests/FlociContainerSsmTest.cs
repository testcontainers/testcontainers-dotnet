using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerSsmTest(FlociContainerFixture fixture)
{
    private AmazonSimpleSystemsManagementClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonSimpleSystemsManagementConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task Ssm_PutAndGetStringParameter_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"/test/param-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.PutParameterAsync(new PutParameterRequest { Name = name, Value = "hello", Type = ParameterType.String }, ct);
        var response = await client.GetParameterAsync(new GetParameterRequest { Name = name }, ct);

        Assert.Equal("hello", response.Parameter.Value);
    }

    [Fact]
    public async Task Ssm_PutAndGetSecureStringParameter_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"/test/secure-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.PutParameterAsync(new PutParameterRequest { Name = name, Value = "secret-value", Type = ParameterType.SecureString }, ct);
        var response = await client.GetParameterAsync(new GetParameterRequest { Name = name, WithDecryption = true }, ct);

        Assert.Equal("secret-value", response.Parameter.Value);
    }

    [Fact]
    public async Task Ssm_GetParametersByPath_ReturnsAllUnderPath()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var path = $"/path-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.PutParameterAsync(new PutParameterRequest { Name = $"{path}/a", Value = "1", Type = ParameterType.String }, ct);
        await client.PutParameterAsync(new PutParameterRequest { Name = $"{path}/b", Value = "2", Type = ParameterType.String }, ct);
        var response = await client.GetParametersByPathAsync(new GetParametersByPathRequest { Path = path, Recursive = true }, ct);

        Assert.Equal(2, response.Parameters.Count);
    }
}
