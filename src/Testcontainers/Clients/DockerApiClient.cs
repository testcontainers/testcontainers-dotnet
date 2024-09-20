namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// A Docker Engine API client.
  /// </summary>
  internal class DockerApiClient
  {
    private static readonly ConcurrentDictionary<Uri, Lazy<IDockerClient>> Clients = new ConcurrentDictionary<Uri, Lazy<IDockerClient>>();

    private static readonly ISet<int> ProcessedHashCodes = new HashSet<int>();

    private static readonly SemaphoreSlim RuntimeInitialized = new SemaphoreSlim(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerApiClient" /> class.
    /// </summary>
    /// <param name="sessionId">The test session id.</param>
    /// <param name="dockerEndpointAuthConfig">The Docker endpoint authentication configuration.</param>
    /// <param name="logger">The logger.</param>
    protected DockerApiClient(Guid sessionId, IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig, ILogger logger)
      : this(Clients.GetOrAdd(dockerEndpointAuthConfig.Endpoint, _ => new Lazy<IDockerClient>(() => GetDockerClient(sessionId, dockerEndpointAuthConfig))).Value, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerApiClient" /> class.
    /// </summary>
    /// <param name="dockerClient">The Docker Engine API client.</param>
    /// <param name="logger">The logger.</param>
    protected DockerApiClient(IDockerClient dockerClient, ILogger logger)
    {
      DockerClient = dockerClient;
      Logger = logger;
    }

    /// <summary>
    /// Gets the Docker Engine API client.
    /// </summary>
    [NotNull]
    protected IDockerClient DockerClient { get; }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    [NotNull]
    protected ILogger Logger { get; }

    /// <summary>
    /// Logs the container runtime information.
    /// </summary>
    /// <remarks>
    /// The method logs the information once per Docker Engine API client and logger.
    /// </remarks>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the information has been logged.</returns>
    public async Task LogContainerRuntimeInfoAsync(CancellationToken ct = default)
    {
      var hashCode = HashCode.Combine(DockerClient, Logger);

      await RuntimeInitialized.WaitAsync(ct)
        .ConfigureAwait(false);

      try
      {
        if (ProcessedHashCodes.Contains(hashCode))
        {
          return;
        }

        var runtimeInfo = new StringBuilder();

        var byteUnits = new[] { "KB", "MB", "GB" };

        var dockerInfo = await DockerClient.System.GetSystemInfoAsync(ct)
          .ConfigureAwait(false);

        var dockerVersion = await DockerClient.System.GetVersionAsync(ct)
          .ConfigureAwait(false);

        runtimeInfo.AppendLine("Connected to Docker:");

        runtimeInfo.Append("  Host: ");
        runtimeInfo.AppendLine(DockerClient.Configuration.EndpointBaseUri.ToString());

        runtimeInfo.Append("  Server Version: ");
        runtimeInfo.AppendLine(dockerInfo.ServerVersion);

        runtimeInfo.Append("  Kernel Version: ");
        runtimeInfo.AppendLine(dockerInfo.KernelVersion);

        runtimeInfo.Append("  API Version: ");
        runtimeInfo.AppendLine(dockerVersion.APIVersion);

        runtimeInfo.Append("  Operating System: ");
        runtimeInfo.AppendLine(dockerInfo.OperatingSystem);

        runtimeInfo.Append("  Total Memory: ");
        runtimeInfo.AppendFormat(CultureInfo.InvariantCulture, "{0:F} {1}", dockerInfo.MemTotal / Math.Pow(1024, byteUnits.Length), byteUnits[byteUnits.Length - 1]);

        var labels = dockerInfo.Labels;
        if (labels != null && labels.Count > 0)
        {
          runtimeInfo.AppendLine();
          runtimeInfo.AppendLine("  Labels: ");
          runtimeInfo.Append(string.Join(Environment.NewLine, labels.Select(label => "    " + label)));
        }
        Logger.LogInformation("{RuntimeInfo}", runtimeInfo);
      }
      catch(Exception e)
      {
        Logger.LogError(e, "Failed to retrieve Docker container runtime information");
      }
      finally
      {
        ProcessedHashCodes.Add(hashCode);
        RuntimeInitialized.Release();
      }
    }

    private static IDockerClient GetDockerClient(Guid sessionId, IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig)
    {
      using (var dockerClientConfiguration = dockerEndpointAuthConfig.GetDockerClientConfiguration(sessionId))
      {
        return dockerClientConfiguration.CreateClient();
      }
    }
  }
}
