namespace Testcontainers.OpenSearch;

[UsedImplicitly]
public sealed class OpenSearchContainerLegacyImagesTest
{
    [Theory]
    [InlineData("opensearchproject/opensearch:1.3.0")]
    [InlineData("opensearchproject/opensearch:2.11.0")]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ShouldConnectWithAdminPassword(string image)
    {
        var opensearchContainer = new OpenSearchBuilder()
            .WithImage(image) // old images have hardcoded admin password
            .Build();
        await opensearchContainer.StartAsync(TestContext.Current.CancellationToken);
        var credentials = opensearchContainer.GetCredentials();
        Assert.Equal("admin", credentials.Password);
        var client = new OpenSearchClient(
            new ConnectionSettings(new Uri(opensearchContainer.GetConnectionString()))
                .BasicAuthentication(credentials.UserName, credentials.SecurePassword)
                .ServerCertificateValidationCallback((_, _, _, _) => true));
        var pingResponse = await client.PingAsync(ct: TestContext.Current.CancellationToken);
        Assert.True(pingResponse.IsValid);
        await opensearchContainer.DisposeAsync();
    }
}