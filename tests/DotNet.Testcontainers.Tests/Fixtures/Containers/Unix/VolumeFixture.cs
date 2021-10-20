namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Volumes;
  using Xunit;

  public sealed class VolumeFixture : IAsyncLifetime
  {
    public IDockerVolume Volume { get; }
      = new TestcontainersVolumeBuilder()
        .WithName("test-volume")
        .Build();

    public Task InitializeAsync()
    {
      return this.Volume.CreateAsync();
    }

    public Task DisposeAsync()
    {
      return this.Volume.DeleteAsync();
    }
  }
}
