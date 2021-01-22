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
    private readonly IDockerVolume volume;

    public VolumeFixture()
    {
      var sessionId = Guid.NewGuid();
      var name = $"testcontainers-volume-{sessionId:D}";
      this.volume = new TestcontainersVolumeBuilder()
        .WithName(name)
        .WithResourceReaperSessionId(sessionId)
        .Build();

      this.SessionId = sessionId;
      this.Name = name;
    }

    public Guid SessionId { get; }

    public string Name { get; }

    public Task InitializeAsync()
    {
      return Task.WhenAll(ResourceReaper.GetAndStartNewAsync(this.SessionId), this.volume.CreateAsync());
    }

    public Task DisposeAsync()
    {
      // The ResourceReaper will take care of the remaining resources.
      return Task.CompletedTask;
    }
  }
}
