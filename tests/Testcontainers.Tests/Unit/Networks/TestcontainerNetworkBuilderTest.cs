namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Networks;
  using Xunit;

  public sealed class TestcontainerNetworkBuilderTest
  {
    [Fact]
    public async Task ShouldCreateNetwork()
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

    [Fact]
    public async Task ShouldCreateNetworkWWithOption()
    {
      // Given
      var networkName = Guid.NewGuid().ToString();
      var networkLabel = Guid.NewGuid().ToString();
      var networkMtuKey = "com.docker.network.driver.mtu";
      var networkMtuValue = "1350";

      // When
      var network = new TestcontainersNetworkBuilder()
        .WithName(networkName)
        .WithLabel("label", networkLabel)
        .WithOption(networkMtuKey, networkMtuValue)
        .Build();

      await network.CreateAsync();

      // Then
      try
      {
        Assert.Equal(networkName, network.Name);
        Assert.IsAssignableFrom<NonExistingDockerNetwork>(network);
        Assert.Equal(networkMtuValue, (network as NonExistingDockerNetwork)!.Options[networkMtuKey]);
      }
      finally
      {
        await network.DeleteAsync();
      }
    }
  }
}
