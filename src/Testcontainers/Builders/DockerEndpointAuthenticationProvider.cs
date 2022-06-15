namespace DotNet.Testcontainers.Builders
{
  using System.Linq;
  using DotNet.Testcontainers.Configurations;

  /// <inheritdoc cref="IAuthenticationProvider{TAuthenticationConfiguration}" />
  internal sealed class DockerEndpointAuthenticationProvider : IAuthenticationProvider<IDockerEndpointAuthenticationConfiguration>
  {
    /// <inheritdoc />
    public bool IsApplicable()
    {
      return true;
    }

    /// <inheritdoc />
    public IDockerEndpointAuthenticationConfiguration GetAuthConfig(string hostname)
    {
      return new IAuthenticationProvider<IDockerEndpointAuthenticationConfiguration>[] { new EnvironmentEndpointAuthenticationProvider(), new NpipeEndpointAuthenticationProvider(), new UnixEndpointAuthenticationProvider() }
        .First(authenticationProvider => authenticationProvider.IsApplicable())
        .GetAuthConfig(hostname);
    }
  }
}
