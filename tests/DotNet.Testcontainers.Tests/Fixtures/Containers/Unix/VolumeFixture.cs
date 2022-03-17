namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Volumes;
  using Xunit;

  public sealed class VolumeFixture : IAsyncLifetime
  {
    private readonly Guid sessionId = Guid.NewGuid();

    private readonly IDockerVolume volume;

    public VolumeFixture()
    {
      this.volume = new TestcontainersVolumeBuilder()
        .WithName(this.SessionId.ToString("D"))
        .WithResourceReaperSessionId(this.SessionId)
        .Build();
    }

    public Guid SessionId
      => this.sessionId;

    public string Name
      => this.volume.Name;

    public Task InitializeAsync()
    {
      return Task.WhenAll(ResourceReaper.GetAndStartNewAsync(this.SessionId), this.volume.CreateAsync());
    }

    public Task DisposeAsync()
    {
      return this.volume.DeleteAsync();
    }
  }
}
