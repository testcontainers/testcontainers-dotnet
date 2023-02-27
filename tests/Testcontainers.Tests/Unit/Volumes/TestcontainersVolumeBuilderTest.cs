namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Volumes;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging.Abstractions;
  using Xunit;

  public sealed class TestcontainersVolumeBuilderTest : IClassFixture<TestcontainersVolumeBuilderTest.DockerVolume>
  {
    private static readonly string VolumeName = Guid.NewGuid().ToString("D");

    private static readonly KeyValuePair<string, string> Label = new KeyValuePair<string, string>(TestcontainersClient.TestcontainersLabel + ".volume.test", Guid.NewGuid().ToString("D"));

    private static readonly KeyValuePair<string, string> ParameterModifier = new KeyValuePair<string, string>(TestcontainersClient.TestcontainersLabel + ".parameter.modifier", Guid.NewGuid().ToString("D"));

    private readonly IVolume volume;

    public TestcontainersVolumeBuilderTest(DockerVolume volume)
    {
      this.volume = volume;
    }

    [Fact]
    public void GetNameThrowsInvalidOperationException()
    {
      _ = Assert.Throws<InvalidOperationException>(() => new VolumeBuilder()
        .WithName(VolumeName)
        .Build()
        .Name);
    }

    [Fact]
    public void GetNameReturnsVolumeName()
    {
      Assert.Equal(VolumeName, this.volume.Name);
    }

    [Fact]
    public async Task CreateVolumeAssignsLabels()
    {
      // Given
      IDockerVolumeOperations volumeOperations = new DockerVolumeOperations(ResourceReaper.DefaultSessionId, TestcontainersSettings.OS.DockerEndpointAuthConfig, NullLogger.Instance);

      // When
      var volumeResponse = await volumeOperations.ByNameAsync(this.volume.Name)
        .ConfigureAwait(false);

      // Then
      Assert.Equal(Label.Value, Assert.Contains(Label.Key, volumeResponse.Labels));
      Assert.Equal(ParameterModifier.Value, Assert.Contains(ParameterModifier.Key, volumeResponse.Labels));
    }

    [UsedImplicitly]
    public sealed class DockerVolume : IVolume, IAsyncLifetime
    {
      private readonly IVolume volume = new VolumeBuilder()
        .WithName(VolumeName)
        .WithLabel(Label.Key, Label.Value)
        .WithCreateParameterModifier(parameterModifier => parameterModifier.Labels.Add(ParameterModifier.Key, ParameterModifier.Value))
        .Build();

      public string Name
      {
        get
        {
          return this.volume.Name;
        }
      }

      public Task InitializeAsync()
      {
        return this.CreateAsync();
      }

      public Task DisposeAsync()
      {
        return this.DeleteAsync();
      }

      public Task CreateAsync(CancellationToken ct = default)
      {
        return this.volume.CreateAsync(ct);
      }

      public Task DeleteAsync(CancellationToken ct = default)
      {
        return this.volume.DeleteAsync(ct);
      }
    }
  }
}
