namespace Testcontainers.RavenDb;

public sealed class RavenDbContainerTest : IAsyncLifetime
{
    private readonly RavenDbContainer _ravenDbContainer = new RavenDbBuilder().Build();

    public Task InitializeAsync()
    {
        return _ravenDbContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _ravenDbContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void GetBuildNumberOperationReturnsBuildNumber()
    {
        // Given
        using var documentStore = new DocumentStore();
        documentStore.Urls = new[] { _ravenDbContainer.GetConnectionString() };
        documentStore.Initialize();

        // When
        var buildNumber = documentStore.Maintenance.Server.Send(new GetBuildNumberOperation());

        // Then
        Assert.Equal("5.4", buildNumber.ProductVersion);
    }
}