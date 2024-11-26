namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using JetBrains.Annotations;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;

  /// <inheritdoc cref="IDockerEndpointAuthenticationProvider" />
  internal class DockerEndpointAuthenticationProvider : IDockerEndpointAuthenticationProvider
  {
    private static readonly TaskFactory TaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

    [CanBeNull]
    public Uri UnavailableEndpoint;

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

                return true;
              }
              catch (Exception)
              {
                UnavailableEndpoint = dockerClientConfiguration.EndpointBaseUri;
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
