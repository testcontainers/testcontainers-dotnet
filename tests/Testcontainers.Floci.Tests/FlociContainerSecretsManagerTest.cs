using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerSecretsManagerTest(FlociContainerFixture fixture)
{
    private AmazonSecretsManagerClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonSecretsManagerConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task SecretsManager_CreateAndRetrieveSecret_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"secret-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.CreateSecretAsync(new CreateSecretRequest { Name = name, SecretString = "my-value" }, ct);
        var response = await client.GetSecretValueAsync(new GetSecretValueRequest { SecretId = name }, ct);

        Assert.Equal("my-value", response.SecretString);
    }

    [Fact]
    public async Task SecretsManager_ListSecrets_ContainsCreatedSecret()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"secret-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.CreateSecretAsync(new CreateSecretRequest { Name = name, SecretString = "value" }, ct);
        var response = await client.ListSecretsAsync(new ListSecretsRequest(), ct);

        Assert.Contains(response.SecretList, s => s.Name == name);
    }
}
