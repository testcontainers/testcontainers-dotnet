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
      return TaskFactory.StartNew(() => IsAvailableAsync(CancellationToken.None), CancellationToken.None)
        .Unwrap()
        .ConfigureAwait(false)
        .GetAwaiter()
        .GetResult();
    }

    /// <summary>
    /// Checks asynchronously whether a connection to the Docker endpoint can be established.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True when a connection to the Docker endpoint can be established, otherwise false.</returns>
    public virtual async Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
      var authConfig = GetAuthConfig();

      if (authConfig == null)
      {
        return false;
      }

      using (var dockerClient = authConfig.GetDockerClientBuilder(ResourceReaper.DefaultSessionId).Build())
      {
        try
        {
          await dockerClient.System.PingAsync(ct)
            .ConfigureAwait(false);

          _cachedException = null;

          return true;
        }
        catch (Exception e)
        {
          var message = $"Failed to connect to Docker endpoint at '{dockerClient.Options.Endpoint}'.";
          _cachedException = new DockerUnavailableException(message, e);

          return false;
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
