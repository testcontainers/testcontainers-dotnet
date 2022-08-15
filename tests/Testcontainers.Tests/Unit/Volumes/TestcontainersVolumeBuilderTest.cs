namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using Xunit;

  public sealed class TestcontainersVolumeBuilderTest
  {
    [Fact]
    public async Task ShouldCreateVolume()
    {
      // Given
      var volumeName = Guid.NewGuid().ToString();

      var volumeLabel = Guid.NewGuid().ToString();

      // When
      var volume = new TestcontainersVolumeBuilder()
        .WithName(volumeName)
        .WithLabel("label", volumeLabel)
        .Build();

      await volume.CreateAsync();

      // Then
      try
      {
        Assert.Equal(volumeName, volume.Name);
      }
      finally
      {
        await volume.DeleteAsync();
      }
    }

    [Fact]
    public void ShouldThrowInvalidOperationException()
    {
      Assert.Throws<InvalidOperationException>(() => new TestcontainersVolumeBuilder()
        .WithName(Guid.Empty.ToString())
        .Build()
        .Name);
    }
  }
}
