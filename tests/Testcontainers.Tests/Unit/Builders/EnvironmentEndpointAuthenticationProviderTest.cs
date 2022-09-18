namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using Xunit;

  public sealed class EnvironmentEndpointAuthenticationProviderTest
  {
    [Theory]
    [ClassData(typeof(AuthConfigTestData))]
    internal void IsApplicable(EnvironmentEndpointAuthenticationProvider provider, bool expectedIsApplicable)
    {
      var actualIsApplicable = provider.IsApplicable();
      Assert.Equal(expectedIsApplicable, actualIsApplicable);
    }

    private sealed class AuthConfigTestData : List<object[]>
    {
      public AuthConfigTestData()
      {
        const string dockerHost = "tcp://127.0.0.1:2375";
        var dockerHostConfigurationWithValue = new PropertiesFileConfiguration(new[] { "docker.host=" + dockerHost });
        var dockerHostConfigurationWithoutValue = new PropertiesFileConfiguration(Array.Empty<string>());
        this.Add(new object[] { new EnvironmentEndpointAuthenticationProvider(null), false });
        this.Add(new object[] { new EnvironmentEndpointAuthenticationProvider(Array.Empty<ICustomConfiguration>()), false });
        this.Add(new object[] { new EnvironmentEndpointAuthenticationProvider(dockerHostConfigurationWithValue), true });
        this.Add(new object[] { new EnvironmentEndpointAuthenticationProvider(dockerHostConfigurationWithoutValue), false });
        this.Add(new object[] { new EnvironmentEndpointAuthenticationProvider(dockerHostConfigurationWithoutValue, dockerHostConfigurationWithValue), true });
      }
    }
  }
}
