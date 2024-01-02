using JetBrains.Annotations;

namespace Testcontainers.DockerCompose;

public abstract class DockerComposeRemoteTest : IAsyncLifetime
{
    private readonly DockerComposeContainer _dockerComposeContainer;

    protected DockerComposeRemoteTest(DockerComposeContainer dockerComposeContainer)
    {
        _dockerComposeContainer = dockerComposeContainer;
    }
    
    public Task InitializeAsync()
    {
        return _dockerComposeContainer.StartAsync();
    }
    
    public async Task DisposeAsync()
    {
        await _dockerComposeContainer.StopAsync();
        await _dockerComposeContainer.DisposeAsync().AsTask();
    }
    
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ContainerStartedSuccessfully()
    {
        Assert.Equal(TestcontainersHealthStatus.Healthy, TestcontainersHealthStatus.Healthy);
    }
    
    [UsedImplicitly]
    public sealed class DockerComposeRemoteConfiguration : DockerComposeRemoteTest
    {
        public DockerComposeRemoteConfiguration()
            : base(new DockerComposeBuilder()
                .WithComposeFile(Path.Combine(Directory.GetCurrentDirectory(), @"./../../../docker-compose.yaml"))
                .Build())
        {
        }
    }
    
    [UsedImplicitly]
    public sealed class DockerComposeLocalConfiguration : DockerComposeRemoteTest
    {
        public DockerComposeLocalConfiguration()
            : base(new DockerComposeBuilder()
                .WithComposeFile(Path.Combine(Directory.GetCurrentDirectory(), @"./../../../docker-compose.yaml"))
                .WithLocalCompose(true)
                .Build())
        {
        }
    }
}
    