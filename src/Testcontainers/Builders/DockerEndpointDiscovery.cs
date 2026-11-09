namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// Discovers the Docker endpoint once and caches the result. The discovery runs
  /// when a resource asks for the endpoint, not when a builder is created.
  /// </summary>
  internal sealed class DockerEndpointDiscovery
  {
    private static readonly TaskFactory TaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

    private readonly Func<IEnumerable<DockerEndpointAuthenticationProvider>> _providers;

    private readonly Lazy<Task<DockerEndpointAuthenticationProvider>> _discovery;

    public DockerEndpointDiscovery(Func<IEnumerable<DockerEndpointAuthenticationProvider>> providers)
    {
      _providers = providers;
      _discovery = new Lazy<Task<DockerEndpointAuthenticationProvider>>(DiscoverAsync);
    }

    public static DockerEndpointDiscovery Default { get; }
      = new DockerEndpointDiscovery(() => TestcontainersSettings.DockerEndpointAuthProviders);

    /// <summary>
    /// Gets a value indicating whether the discovery has been started.
    /// </summary>
    public bool IsStarted => _discovery.IsValueCreated;

    /// <summary>
    /// Gets the first applicable and available provider, or null if there is none.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The provider or null.</returns>
    [ItemCanBeNull]
    public async Task<DockerEndpointAuthenticationProvider> GetProviderAsync(CancellationToken ct = default)
    {
      var provider = await _discovery.Value
        .ConfigureAwait(false);

      ct.ThrowIfCancellationRequested();
      return provider;
    }

    /// <summary>
    /// Gets the provider synchronously. Prefer <see cref="GetProviderAsync" />. This
    /// path only exists for members that cannot be asynchronous.
    /// </summary>
    /// <returns>The provider or null.</returns>
    [CanBeNull]
    public DockerEndpointAuthenticationProvider GetProvider()
    {
      var discovery = _discovery.Value;

      if (discovery.IsCompleted)
      {
        return discovery.GetAwaiter().GetResult();
      }

      return TaskFactory.StartNew(() => discovery, CancellationToken.None)
        .Unwrap()
        .ConfigureAwait(false)
        .GetAwaiter()
        .GetResult();
    }

    public Exception CreateDockerUnavailableException()
    {
      var unavailableExceptions = _providers()
        .Select(authProvider => authProvider.LastException)
        .Where(exception => exception != null);

      var exception = new AggregateException(unavailableExceptions);

      var exceptionInfo = new StringBuilder(512);
      exceptionInfo.AppendLine("Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.");
      exceptionInfo.AppendLine("You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.");
      exceptionInfo.AppendLine("For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.");
      exceptionInfo.AppendLine("  Details: ");
      exceptionInfo.Append(string.Join(Environment.NewLine, exception.InnerExceptions.Select(e => "    " + e.Message)));

      return new DockerUnavailableException(exceptionInfo.ToString(), exception);
    }

    private async Task<DockerEndpointAuthenticationProvider> DiscoverAsync()
    {
      // Do not capture the caller's context, the discovery is shared by all callers.
      await Task.Yield();

      foreach (var provider in _providers())
      {
        if (provider.IsApplicable() && await provider.IsAvailableAsync(CancellationToken.None).ConfigureAwait(false))
        {
          return provider;
        }
      }

      return null;
    }
  }
}
