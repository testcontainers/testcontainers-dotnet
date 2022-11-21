namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;

  /// <inheritdoc cref="IDockerEndpointAuthenticationProvider" />
  internal class DockerEndpointAuthenticationProvider : IDockerEndpointAuthenticationProvider
  {
    private static readonly TaskFactory TaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

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

      using (var dockerClientConfiguration = authConfig.GetDockerClientConfiguration(ResourceReaper.DefaultSessionId))
      {
        using (var dockerClient = dockerClientConfiguration.CreateClient())
        {
          try
          {
            TaskFactory.StartNew(() => dockerClient.System.PingAsync())
              .Unwrap()
              .ConfigureAwait(false)
              .GetAwaiter()
              .GetResult();

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
