namespace Testcontainers.RavenDb;

public sealed class RavenDbContainerTest : ContainerTest<RavenDbBuilder, RavenDbContainer>
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void GetBuildNumberOperationReturnsBuildNumber()
    {
        // Given
        using var documentStore = new DocumentStore();
        documentStore.Urls = new[] { Container.GetConnectionString() };
        documentStore.Initialize();

        // When
        var buildNumber = documentStore.Maintenance.Server.Send(new GetBuildNumberOperation());

        // Then
        Assert.Equal("5.4", buildNumber.ProductVersion);
    }
}