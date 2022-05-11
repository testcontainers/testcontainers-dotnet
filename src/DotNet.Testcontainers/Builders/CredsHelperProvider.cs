namespace DotNet.Testcontainers.Builders
{
  using DotNet.Testcontainers.Configurations;

  /// <inheritdoc cref="IAuthenticationProvider" />
  internal sealed class CredsHelperProvider : IAuthenticationProvider
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
