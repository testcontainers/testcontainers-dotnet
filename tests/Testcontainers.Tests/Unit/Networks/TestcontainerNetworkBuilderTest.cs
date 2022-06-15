namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using Xunit;

  public sealed class TestcontainerNetworkBuilderTest
  {
    [Fact]
    public async Task ShouldNetworkVolume()
    {
      // Given
      var networkName = Guid.NewGuid().ToString();

      var networkLabel = Guid.NewGuid().ToString();

      // When
      var network = new TestcontainersNetworkBuilder()
        .WithName(networkName)
        .WithLabel("label", networkLabel)
        .Build();

      await network.CreateAsync();

      // Then
      try
      {
        Assert.Equal(networkName, network.Name);
      }
      finally
      {
        await network.DeleteAsync();
      }
    }

    [Fact]
    public void ShouldThrowInvalidOperationException()
    {
      Assert.Throws<InvalidOperationException>(() => new TestcontainersNetworkBuilder()
        .WithName(Guid.Empty.ToString())
        .Build()
        .Name);
    }
  }
}
