namespace DotNet.Testcontainers.Builders
{
  using System.Linq;
  using DotNet.Testcontainers.Configurations;

  /// <inheritdoc cref="IDockerEndpointAuthenticationProvider" />
  internal sealed class DockerEndpointAuthenticationProvider : IDockerEndpointAuthenticationProvider
  {
    /// <inheritdoc />
    public bool IsApplicable()
    {
      return true;
    }

    /// <inheritdoc />
    public IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      return new IDockerEndpointAuthenticationProvider[] { new EnvironmentEndpointAuthenticationProvider(), new NpipeEndpointAuthenticationProvider(), new UnixEndpointAuthenticationProvider() }
        .First(authenticationProvider => authenticationProvider.IsApplicable())
        .GetAuthConfig();
    }
  }
}
