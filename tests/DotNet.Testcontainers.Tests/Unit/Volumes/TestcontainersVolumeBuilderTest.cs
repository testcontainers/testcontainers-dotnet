namespace DotNet.Testcontainers.Tests.Unit.Volumes
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using Xunit;

  public class TestcontainersVolumeBuilderTest
  {
    [Fact]
    public async Task CreateTest()
    {
      // Given
      var volumeName = Guid.NewGuid().ToString();
      var volumeLabel = Guid.NewGuid().ToString();

      // When
      await using var volume = new TestcontainersVolumeBuilder()
        .WithName(volumeName)
        .WithLabel("label", volumeLabel)
        .Build();
      await volume.CreateAsync();

      // Then
      Assert.Equal(volumeName, volume.Name);
      Assert.Equal(1, volume.Labels.Count);
      Assert.Equal(volumeLabel, volume.Labels.First().Value);
    }

    [Fact]
    public async Task NotCreatedTest()
    {
      // Given
      var volumeName = Guid.NewGuid().ToString();
      var volumeLabel = Guid.NewGuid().ToString();

      // When
      await using var volume = new TestcontainersVolumeBuilder()
        .WithName(volumeName)
        .WithLabel("label", volumeLabel)
        .Build();

      Assert.Throws<InvalidOperationException>(() => volume.Name);
      Assert.Throws<InvalidOperationException>(() => volume.Labels);
    }
  }
}
