namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Collections.Generic;
  using System.Linq;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using Xunit;

  public static class ContainerConfigurationConverterTest
  {
    private const string Port = "2375";

    public sealed class ExposedPorts
    {
      [Fact]
      public void ShouldAddTcpPortSuffix()
      {
        // Given
        var containerConfiguration = new ContainerConfiguration(exposedPorts: new Dictionary<string, string> { { Port, null } });

        // When
        var exposedPort = new ContainerConfigurationConverter(containerConfiguration).ExposedPorts.Single().Key;

        // Then
        Assert.Equal($"{Port}/tcp", exposedPort);
      }

      [Theory]
      [InlineData("UDP")]
      [InlineData("TCP")]
      [InlineData("SCTP")]
      public void ShouldKeepPortSuffix(string portSuffix)
      {
        // Given
        var qualifiedPort = $"{Port}/{portSuffix}";

        var containerConfiguration = new ContainerConfiguration(exposedPorts: new Dictionary<string, string> { { qualifiedPort, null } });

        // When
        var exposedPort = new ContainerConfigurationConverter(containerConfiguration).ExposedPorts.Single().Key;

        // Then
        Assert.Equal($"{Port}/{portSuffix}".ToLowerInvariant(), exposedPort);
      }
    }

    public sealed class PortBindings
    {
      [Fact]
      public void ShouldAddTcpPortSuffix()
      {
        // Given
        var containerConfiguration = new ContainerConfiguration(portBindings: new Dictionary<string, string> { { Port, Port } });

        // When
        var portBinding = new ContainerConfigurationConverter(containerConfiguration).PortBindings.Single().Key;

        // Then
        Assert.Equal($"{Port}/tcp", portBinding);
      }

      [Theory]
      [InlineData("UDP")]
      [InlineData("TCP")]
      [InlineData("SCTP")]
      public void ShouldKeepPortSuffix(string portSuffix)
      {
        // Given
        var qualifiedPort = $"{Port}/{portSuffix}";

        var containerConfiguration = new ContainerConfiguration(portBindings: new Dictionary<string, string> { { qualifiedPort, Port } });

        // When
        var portBinding = new ContainerConfigurationConverter(containerConfiguration).PortBindings.Single().Key;

        // Then
        Assert.Equal($"{Port}/{portSuffix}".ToLowerInvariant(), portBinding);
      }
    }
  }
}
