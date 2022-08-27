namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Volumes;
  using JetBrains.Annotations;
  using Xunit;

  [UsedImplicitly]
  public sealed class VolumeFixture : IAsyncLifetime
  {
    private readonly IDockerVolume volume;

    public VolumeFixture()
    {
      this.volume = new TestcontainersVolumeBuilder()
        .WithName(this.SessionId.ToString("D"))
        .WithResourceReaperSessionId(this.SessionId)
        .Build();
    }

    public Guid SessionId { get; }
      = Guid.NewGuid();

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
