namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Volumes;
  using JetBrains.Annotations;
  using Xunit;

  [UsedImplicitly]
  public sealed class VolumeFixture : IAsyncLifetime
  {
    public IVolume Volume { get; }
      = new VolumeBuilder()
        .WithName(Guid.NewGuid().ToString("D"))
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
