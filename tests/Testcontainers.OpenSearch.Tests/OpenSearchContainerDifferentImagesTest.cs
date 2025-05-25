namespace Testcontainers.OpenSearch;

[UsedImplicitly]
public sealed class OpenSearchContainerDifferentImagesTest
{
    [Theory]
    [InlineData("opensearchproject/opensearch:1.3.0")]
    [InlineData("opensearchproject/opensearch:2.0.0")]
    [InlineData("opensearchproject/opensearch:2.11.0")]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ShouldConnectWithAdminPassword(string image)
    {
        // <!-- -8<- [start:LegacyImageAdminPassword] -->
        var opensearchContainer = new OpenSearchBuilder()
            .WithImage(image)
            .WithPassword(OpenSearchBuilder.DefaultOldInsecurePassword) // old images have hardcoded admin password
            .Build();
        // <!-- -8<- [end:LegacyImageAdminPassword] -->
        await opensearchContainer.StartAsync(TestContext.Current.CancellationToken);
        var credentials = opensearchContainer.GetConnectionCredentials();
        Assert.Equal(OpenSearchBuilder.DefaultOldInsecurePassword, credentials.Password); // check that creds have 'admin' pw set
        var client = new OpenSearchClient(
            new ConnectionSettings(new Uri(opensearchContainer.GetConnection()))
                .BasicAuthentication(credentials.UserName, credentials.SecurePassword)
                .ServerCertificateValidationCallback((_, _, _, _) => true));
        var pingResponse = await client.PingAsync(ct: TestContext.Current.CancellationToken);
        Assert.True(pingResponse.IsValid);
        await opensearchContainer.DisposeAsync();
    }

    [Theory]
    [InlineData("opensearchproject/opensearch:2.12.0", "CustomVeryStrongP@ssw0rd!")]
    [InlineData("opensearchproject/opensearch:3.0.0", "CustomVeryStrongP@ssw0rd!")]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ShouldConnectWithCustomAdminPassword(string image, string password)
    {
        var opensearchContainer = new OpenSearchBuilder()
            .WithImage(image)
            .WithPassword(password) // newer images can use custom admin password
            .Build();
        await opensearchContainer.StartAsync(TestContext.Current.CancellationToken);
        var credentials = opensearchContainer.GetConnectionCredentials();
        Assert.Equal(password, credentials.Password); // check that creds have custom pw set
        var client = new OpenSearchClient(
            new ConnectionSettings(new Uri(opensearchContainer.GetConnection()))
                .BasicAuthentication(credentials.UserName, credentials.SecurePassword)
                .ServerCertificateValidationCallback((_, _, _, _) => true));
        var pingResponse = await client.PingAsync(ct: TestContext.Current.CancellationToken);
        Assert.True(pingResponse.IsValid);
        await opensearchContainer.DisposeAsync();
    }
}