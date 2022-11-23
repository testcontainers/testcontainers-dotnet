namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Globalization;
  using System.Linq;
  using System.Net;
  using System.Net.Sockets;
  using System.Runtime.InteropServices;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// This class represents the Testcontainers settings.
  /// </summary>
  [PublicAPI]
  public static class TestcontainersSettings
  {
    private static readonly IDockerImage RyukContainerImage = new DockerImage("testcontainers/ryuk:0.3.4");

    private static readonly ManualResetEventSlim ManualResetEvent = new ManualResetEventSlim(false);

    private static readonly IDockerEndpointAuthenticationConfiguration DockerEndpointAuthConfig =
      new IDockerEndpointAuthenticationProvider[]
        {
          new MTlsEndpointAuthenticationProvider(),
          new TlsEndpointAuthenticationProvider(),
          new EnvironmentEndpointAuthenticationProvider(),
          new NpipeEndpointAuthenticationProvider(),
          new UnixEndpointAuthenticationProvider(),
        }
        .AsParallel()
        .Where(authProvider => authProvider.IsApplicable())
        .Where(authProvider => authProvider.IsAvailable())
        .Select(authProvider => authProvider.GetAuthConfig())
        .FirstOrDefault();

    static TestcontainersSettings()
    {
      Task.Run(async () =>
      {
        var runtimeInfo = new StringBuilder();

        if (DockerEndpointAuthConfig != null)
        {
          using (var dockerClientConfiguration = DockerEndpointAuthConfig.GetDockerClientConfiguration())
          {
            using (var dockerClient = dockerClientConfiguration.CreateClient())
            {
              try
              {
                var byteUnits = new[] { "KB", "MB", "GB" };

                var dockerInfo = await dockerClient.System.GetSystemInfoAsync()
                  .ConfigureAwait(false);

                var dockerVersion = await dockerClient.System.GetVersionAsync()
                  .ConfigureAwait(false);

                runtimeInfo.AppendLine("Connected to Docker:");

                runtimeInfo.Append("  Host: ");
                runtimeInfo.AppendLine(dockerClient.Configuration.EndpointBaseUri.ToString());

                runtimeInfo.Append("  Server Version: ");
                runtimeInfo.AppendLine(dockerInfo.ServerVersion);

                runtimeInfo.Append("  Kernel Version: ");
                runtimeInfo.AppendLine(dockerInfo.KernelVersion);

                runtimeInfo.Append("  API Version: ");
                runtimeInfo.AppendLine(dockerVersion.APIVersion);

                runtimeInfo.Append("  Operating System: ");
                runtimeInfo.AppendLine(dockerInfo.OperatingSystem);

                runtimeInfo.Append("  Total Memory: ");
                runtimeInfo.AppendFormat(CultureInfo.InvariantCulture, "{0:F} {1}", dockerInfo.MemTotal / Math.Pow(1024, byteUnits.Length), byteUnits.Last());
              }
              catch
              {
                // Ignore exceptions in auto discovery. Users can provide the Docker endpoint with the builder too.
              }
            }
          }
        }
        else
        {
          runtimeInfo.AppendLine("Auto discovery did not detect a Docker host configuration");
        }

#pragma warning disable CA1848, CA2254
        Logger.LogInformation(runtimeInfo.ToString());
#pragma warning restore CA1848, CA2254
        ManualResetEvent.Set();
      });
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="ResourceReaper" /> is enabled or not.
    /// </summary>
    [PublicAPI]
    public static bool ResourceReaperEnabled { get; set; }
      = !PropertiesFileConfiguration.Instance.GetRyukDisabled() && !EnvironmentConfiguration.Instance.GetRyukDisabled();

    /// <summary>
    /// Gets or sets the <see cref="ResourceReaper" /> image.
    /// </summary>
    [PublicAPI]
    public static IDockerImage ResourceReaperImage { get; set; }
      = PropertiesFileConfiguration.Instance.GetRyukContainerImage() ?? EnvironmentConfiguration.Instance.GetRyukContainerImage() ?? RyukContainerImage;

    /// <summary>
    /// Gets or sets the <see cref="ResourceReaper" /> public host port.
    /// </summary>
    /// <remarks>
    /// The <see cref="ResourceReaper" /> might not be able to connect to Ryuk on Docker Desktop for Windows.
    /// Assigning a random port might run into the excluded port range. The container starts, but we cannot establish a TCP connection:
    /// - https://github.com/docker/for-win/issues/3171.
    /// - https://github.com/docker/for-win/issues/11584.
    /// </remarks>
    [PublicAPI]
    public static Func<IDockerEndpointAuthenticationConfiguration, ushort> ResourceReaperPublicHostPort { get; set; }
      = GetResourceReaperPublicHostPort;

    /// <summary>
    /// Gets or sets a prefix that applies to every image that is pulled from Docker Hub.
    /// </summary>
    /// <remarks>
    /// Please verify that all required images exist in your registry.
    /// </remarks>
    [PublicAPI]
    [CanBeNull]
    public static string HubImageNamePrefix { get; set; }
      = PropertiesFileConfiguration.Instance.GetHubImageNamePrefix() ?? EnvironmentConfiguration.Instance.GetHubImageNamePrefix();

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [PublicAPI]
    [NotNull]
    public static ILogger Logger { get; set; }
      = new Logger();

    /// <summary>
    /// Gets or sets the host operating system.
    /// </summary>
    [PublicAPI]
    [NotNull]
    public static IOperatingSystem OS { get; set; }
      = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? (IOperatingSystem)new Windows(DockerEndpointAuthConfig) : new Unix(DockerEndpointAuthConfig);

    /// <summary>
    /// Gets the wait handle that signals settings initialized.
    /// </summary>
    internal static WaitHandle SettingsInitialized
      => ManualResetEvent.WaitHandle;

    private static ushort GetResourceReaperPublicHostPort(IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig)
    {
      // Let Docker choose the random public host port. This includes Docker Engines exposed via TCP (Docker Desktop for Windows).
      if (!NpipeEndpointAuthenticationProvider.DockerEngine.Equals(dockerEndpointAuthConfig.Endpoint))
      {
        return 0;
      }

      // Get the next available public host port. This might run in rare cases into a race-condition.
      // It's still much more stable than letting Docker Desktop for Windows choose the port.
      using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
      {
        socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        return (ushort)((IPEndPoint)socket.LocalEndPoint).Port;
      }
    }
  }
}
