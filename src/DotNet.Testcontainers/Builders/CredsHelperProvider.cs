namespace DotNet.Testcontainers.Builders
{
  using DotNet.Testcontainers.Configurations;

  /// <inheritdoc cref="IAuthenticationProvider{TAuthenticationConfiguration}" />
  internal sealed class CredsHelperProvider : IAuthenticationProvider<IDockerRegistryAuthenticationConfiguration>
  {
    /// <inheritdoc />
    public bool IsApplicable()
    {
      return false;
    }

    /// <inheritdoc />
    public IDockerRegistryAuthenticationConfiguration GetAuthConfig(string hostname)
    {
      return null;
    }
  }
}
