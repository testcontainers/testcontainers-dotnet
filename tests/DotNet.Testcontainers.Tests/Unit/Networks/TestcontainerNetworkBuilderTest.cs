namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using Xunit;

  public class TestcontainerNetworkBuilderTest
  {
    [Fact]
    public async Task CreateTest()
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

      try
      {
        // Then
        Assert.Equal(networkName, network.Name);
      }
      finally
      {
        await network.DeleteAsync();
      }
    }

    [Fact]
    public void NotCreatedTest()
    {
      // Given
      var networkName = Guid.NewGuid().ToString();
      var networkLabel = Guid.NewGuid().ToString();

      // When
      var network = new TestcontainersNetworkBuilder()
        .WithName(networkName)
        .WithLabel("label", networkLabel)
        .Build();

      Assert.Throws<InvalidOperationException>(() => network.Name);
    }
  }
}
