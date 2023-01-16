namespace DotNet.Testcontainers.ResourceReaper.Tests
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class ResourceReaperPruneTest : IAsyncLifetime
  {
    private readonly Guid id = Guid.NewGuid();

    private readonly string name = Guid.NewGuid().ToString("D");

    public ResourceReaperPruneTest()
    {
      File.WriteAllText(Path.Combine(TestSession.TempDirectoryPath, this.name), "FROM scratch");
    }

    public Task InitializeAsync()
    {
      var container = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage(CommonImages.Alpine)
        .WithEntrypoint(CommonCommands.SleepInfinity)
        .WithResourceReaperSessionId(this.id)
        .WithName(this.name)
        .Build();

      var image = new ImageFromDockerfileBuilder()
        .WithResourceReaperSessionId(this.id)
        .WithDockerfileDirectory(TestSession.TempDirectoryPath)
        .WithDockerfile(this.name)
        .WithName(this.name)
        .Build();

      var network = new TestcontainersNetworkBuilder()
        .WithResourceReaperSessionId(this.id)
        .WithName(this.name)
        .Build();

      var volume = new TestcontainersVolumeBuilder()
        .WithResourceReaperSessionId(this.id)
        .WithName(this.name)
        .Build();

      return Task.WhenAll(container.StartAsync(), image.CreateAsync(), network.CreateAsync(), volume.CreateAsync());
    }

    public Task DisposeAsync()
    {
      return Task.CompletedTask;
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ResourceReaperPrunesDockerResource()
    {
      // Given
      var resourceReaper = await ResourceReaper.GetAndStartNewAsync(this.id, TestcontainersSettings.OS.DockerEndpointAuthConfig, CommonImages.Ryuk, ResourceReaper.UnixSocketMount.Instance)
        .ConfigureAwait(false);

      // When
      await resourceReaper.DisposeAsync()
        .ConfigureAwait(false);

      // Then
      Assert.False(DockerCli.ResourceExists(DockerCli.DockerResource.Container, this.name));
      Assert.False(DockerCli.ResourceExists(DockerCli.DockerResource.Network, this.name));
      Assert.False(DockerCli.ResourceExists(DockerCli.DockerResource.Volume, this.name));
      Assert.False(DockerCli.ResourceExists(DockerCli.DockerResource.Image, this.name));
    }
  }
}
