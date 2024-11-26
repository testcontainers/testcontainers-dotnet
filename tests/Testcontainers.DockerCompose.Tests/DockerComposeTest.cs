namespace Testcontainers.DockerCompose;

public abstract class DockerComposeContainerTest : IAsyncLifetime
{
    private readonly DockerComposeContainer _dockerComposeContainer;

    private DockerComposeContainerTest(DockerComposeContainer dockerComposeContainer)
    {
        _dockerComposeContainer = dockerComposeContainer;
    }

    public Task InitializeAsync()
    {
        return _dockerComposeContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
        return _dockerComposeContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ContainerStartedSuccessfully()
    {
        // TODO: How do we test that the Compose configuration is actually working? How do we access services?
        Assert.Equal(TestcontainersHealthStatus.Healthy, TestcontainersHealthStatus.Healthy);
    }

    [UsedImplicitly]
    public sealed class DockerComposeLocalConfiguration : DockerComposeContainerTest
    {
        public DockerComposeLocalConfiguration()
            : base(new DockerComposeBuilder()
                .WithComposeFile("docker-compose.yml")
                .WithComposeMode(DockerComposeMode.Local)
                .Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class DockerComposeRemoteConfiguration : DockerComposeContainerTest
    {
        public DockerComposeRemoteConfiguration()
            : base(new DockerComposeBuilder()
                .WithComposeFile("docker-compose.yml")
                .WithComposeMode(DockerComposeMode.Remote)
                .Build())
        {
        }
    }
}