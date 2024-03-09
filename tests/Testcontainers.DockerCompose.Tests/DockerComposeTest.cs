namespace Testcontainers.DockerCompose;

public abstract class DockerComposeTest : IAsyncLifetime
{
    private readonly DockerComposeContainer _dockerComposeContainer;

    protected DockerComposeTest(DockerComposeContainer dockerComposeContainer)
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
    public sealed class DockerComposeRemoteConfiguration : DockerComposeTest
    {
        public DockerComposeRemoteConfiguration()
            : base(new DockerComposeBuilder()
                .WithComposeFile(Path.Combine(Directory.GetCurrentDirectory(), @"./../../../docker-compose.yaml"))
                .Build())
        {
        }
    }
    
    [UsedImplicitly]
    public sealed class DockerComposeLocalConfiguration : DockerComposeTest
    {
        public DockerComposeLocalConfiguration()
            : base(new DockerComposeBuilder()
                .WithComposeFile(Path.Combine(Directory.GetCurrentDirectory(), @"./../../../docker-compose.yaml"))
                .WithLocalCompose(true)
                .Build())
        {
        }
    }
    
    [UsedImplicitly]
    public sealed class DockerComposeRemoteWithOptionConfiguration : DockerComposeTest
    {
        public DockerComposeRemoteWithOptionConfiguration()
            : base(new DockerComposeBuilder()
                .WithComposeFile(Path.Combine(Directory.GetCurrentDirectory(), @"./../../../docker-compose.yaml"))
                .WithOptions("--compatibility")
                .Build())
        {
        }
    }
    
    [UsedImplicitly]
    public sealed class DockerComposeLocalWithOptionConfiguration : DockerComposeTest
    {
        public DockerComposeLocalWithOptionConfiguration()
            : base(new DockerComposeBuilder()
                .WithComposeFile(Path.Combine(Directory.GetCurrentDirectory(), @"./../../../docker-compose.yaml"))
                .WithOptions("--compatibility")
                .Build())
        {
        }
    }
    
    [UsedImplicitly]
    public sealed class DockerComposeRemoteWithRemoveImagesConfiguration : DockerComposeTest
    {
        public DockerComposeRemoteWithRemoveImagesConfiguration()
            : base(new DockerComposeBuilder()
                .WithComposeFile(Path.Combine(Directory.GetCurrentDirectory(), @"./../../../docker-compose-rmi.yaml"))
                .WithRemoveImages(RemoveImages.All)
                .Build())
        {
        }
    }
    
    [UsedImplicitly]
    public sealed class DockerComposeLocalWithRemoveImagesConfiguration : DockerComposeTest
    {
        public DockerComposeLocalWithRemoveImagesConfiguration()
            : base(new DockerComposeBuilder()
                .WithComposeFile(Path.Combine(Directory.GetCurrentDirectory(), @"./../../../docker-compose-rmi.yaml"))
                .WithRemoveImages(RemoveImages.All)
                .WithLocalCompose(true)
                .Build())
        {
        }
    }
}
    