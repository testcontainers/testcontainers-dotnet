namespace DotNet.Testcontainers.Builders
{
  using System;
  using DotNet.Testcontainers.Configurations;

  /// <inheritdoc cref="IDockerEndpointAuthenticationProvider" />
  internal class DockerEndpointAuthenticationProvider : IDockerEndpointAuthenticationProvider
  {
    /// <inheritdoc />
    public virtual bool IsApplicable()
    {
      return false;
    }

    /// <inheritdoc />
    public virtual bool IsAvailable()
    {
      var authConfig = this.GetAuthConfig();

      if (authConfig == null)
      {
        return false;
      }

      using (var dockerClientConfiguration = authConfig.GetDockerClientConfiguration())
      {
        using (var dockerClient = dockerClientConfiguration.CreateClient())
        {
          try
          {
            dockerClient.System.PingAsync().GetAwaiter().GetResult();
            return true;
          }
          catch (Exception)
          {
            return false;
          }
        }
      }
    }

    /// <inheritdoc />
    public virtual IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      return null;
    }
  }
}
