namespace DotNet.Testcontainers.Tests.Unit.Internals.Mapper
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;

  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Internals.Mappers;

  using Xunit;

  public class TestcontainersConfigurationConverterTest
  {
    #region Exposed ports

    [Fact]
    public void ExposedPortsUdpSuffixIsKept()
    {
      const int port = 7878;
      var qualifiedPort = $"{port}/uDp";
      RunExposedPortsTest(qualifiedPort, qualifiedPort.ToLowerInvariant());
    }

    [Fact]
    public void ExposedPortsTcpSuffixIsKept()
    {
      const int port = 7878;
      var qualifiedPort = $"{port}/tcP";
      RunExposedPortsTest(qualifiedPort, qualifiedPort.ToLowerInvariant());
    }

    [Fact]
    public void ExposedPortTcpSuffixIsAdded()
    {
      const int port = 7878;
      RunExposedPortsTest(port.ToString(CultureInfo.InvariantCulture), $"{port}/tcp");
    }

    private static void RunExposedPortsTest(string port, string gauge)
    {
      var testcontainersConfiguration = GetTestcontainersConfiguration(
        new Dictionary<string, string> { { port, port } },
        null);

      var testcontainersConfigurationConverter = new TestcontainersConfigurationConverter
      (testcontainersConfiguration);

      Assert.NotNull(testcontainersConfigurationConverter.ExposedPorts);
      Assert.Equal(1, testcontainersConfigurationConverter.ExposedPorts.Count);
      Assert.True(
        testcontainersConfigurationConverter
          .ExposedPorts
          .Keys
          .First()
          .Equals(gauge, StringComparison.Ordinal));
    }

    #endregion
    #region Port bindings

    [Fact]
    public void PortBindingsUdpSuffixIsKept()
    {
      const int port = 7878;
      var qualifiedPort = $"{port}/uDp";

      RunPortBindingsTest(
        port.ToString(CultureInfo.InvariantCulture),
        qualifiedPort,
        qualifiedPort.ToLowerInvariant());
    }

    [Fact]
    public void PortBindingsTcpSuffixIsKept()
    {
      const int port = 7878;
      var qualifiedPort = $"{port}/tcP";

      RunPortBindingsTest(
        port.ToString(CultureInfo.InvariantCulture),
        qualifiedPort,
        qualifiedPort.ToLowerInvariant());
    }

    [Fact]
    public void PortBindingsTcpSuffixIsAdded()
    {
      var port = "7878";
      RunPortBindingsTest(port, port, $"{port}/tcp");
    }

    private static void RunPortBindingsTest(string hostPort, string dockerPort, string dockerPortGauge)
    {
      var testcontainersConfiguration = GetTestcontainersConfiguration(
        null,
        new Dictionary<string, string> { { dockerPort, hostPort } });

      var testcontainersConfigurationConverter = new TestcontainersConfigurationConverter
      (testcontainersConfiguration);

      var portBindings = testcontainersConfigurationConverter.PortBindings;

      Assert.NotNull(portBindings);
      Assert.Equal(1, portBindings.Count);

      Assert.NotNull(portBindings.Values);
      Assert.Equal(1, portBindings.Values.Count);

      var key = portBindings.Keys.First();
      var value = portBindings.Values.First().First();

      Assert.True(key.Equals(dockerPortGauge, StringComparison.Ordinal));
      Assert.True(value.HostPort.Equals(hostPort, StringComparison.Ordinal));
    }

    #endregion

    private static TestcontainersConfiguration GetTestcontainersConfiguration(
      IReadOnlyDictionary<string, string> exposedPorts,
      IReadOnlyDictionary<string, string> portBindings)
    {
      return new TestcontainersConfiguration
            (
              default,
              default,
              default,
              default,
              default,
              default,
              default,
              default,
              default,
              default,
              exposedPorts,
              portBindings,
              default,
              default,
              default,
              default,
              default
            );
    }
  }
}
