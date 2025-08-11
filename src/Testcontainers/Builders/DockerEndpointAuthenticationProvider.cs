namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IDockerEndpointAuthenticationProvider" />
  internal class DockerEndpointAuthenticationProvider : IDockerEndpointAuthenticationProvider
  {
    private static readonly TaskFactory TaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

    [CanBeNull]
    private Exception _cachedException;

    /// <summary>
    /// Exposes the exception that occurred during the last Docker availability check.
    /// </summary>
    [CanBeNull]
    public Exception LastException => _cachedException;

    /// <inheritdoc />
    public virtual bool IsApplicable()
    {
      return false;
    }

    /// <inheritdoc />
    public virtual bool IsAvailable()
    {
      var authConfig = GetAuthConfig();

      if (authConfig == null)
      {
        return false;
      }

      return TaskFactory.StartNew(async () =>
        {
          using (var dockerClientConfiguration = authConfig.GetDockerClientConfiguration(ResourceReaper.DefaultSessionId))
          {
            using (var dockerClient = dockerClientConfiguration.CreateClient())
            {
              try
              {
                await dockerClient.System.PingAsync()
                  .ConfigureAwait(false);

                _cachedException = null;

                return true;
              }
              catch (Exception e)
              {
                var message = $"Failed to connect to Docker endpoint at '{dockerClientConfiguration.EndpointBaseUri}'.";
                _cachedException = new DockerUnavailableException(message, e);

                return false;
              }
            }
          }
        })
        .Unwrap()
        .ConfigureAwait(false)
        .GetAwaiter()
        .GetResult();
    }

    /// <inheritdoc />
    public virtual IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      return null;
    }
  }
}
