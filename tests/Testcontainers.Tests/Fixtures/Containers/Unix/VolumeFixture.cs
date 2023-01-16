namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using JetBrains.Annotations;
  using Xunit;

  [UsedImplicitly]
  public sealed class VolumeFixture : IAsyncLifetime
  {
    private readonly IDockerVolume volume = new TestcontainersVolumeBuilder()
      .WithName(Guid.NewGuid().ToString("D"))
      .Build();

    public string Name
      => this.volume.Name;

    public Task InitializeAsync()
    {
      return this.volume.CreateAsync();
    }

    public Task DisposeAsync()
    {
      return this.volume.DeleteAsync();
    }
  }
}
