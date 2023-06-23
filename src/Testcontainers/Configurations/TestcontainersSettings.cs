namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
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
    private static readonly ManualResetEventSlim ManualResetEvent = new ManualResetEventSlim(false);

    [CanBeNull]
    private static readonly IDockerEndpointAuthenticationProvider DockerEndpointAuthProvider
      = new IDockerEndpointAuthenticationProvider[]
        {
          new TestcontainersEndpointAuthenticationProvider(),
          new MTlsEndpointAuthenticationProvider(),
          new TlsEndpointAuthenticationProvider(),
          new EnvironmentEndpointAuthenticationProvider(),
          new NpipeEndpointAuthenticationProvider(),
          new UnixEndpointAuthenticationProvider(),
          new DockerDesktopEndpointAuthenticationProvider(),
          new RootlessUnixEndpointAuthenticationProvider(),
        }
        .Where(authProvider => authProvider.IsApplicable())
        .FirstOrDefault(authProvider => authProvider.IsAvailable());

    [CanBeNull]
    private static readonly IDockerEndpointAuthenticationConfiguration DockerEndpointAuthConfig
      = DockerEndpointAuthProvider?.GetAuthConfig();

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
                runtimeInfo.AppendFormat(CultureInfo.InvariantCulture, "{0:F} {1}", dockerInfo.MemTotal / Math.Pow(1024, byteUnits.Length), byteUnits[byteUnits.Length - 1]);
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
    /// Gets or sets the Docker host override value.
    /// </summary>
    [CanBeNull]
    public static string DockerHostOverride { get; set; }
      = DockerEndpointAuthProvider is ICustomConfiguration config
        ? config.GetDockerHostOverride() : EnvironmentConfiguration.Instance.GetDockerHostOverride() ?? PropertiesFileConfiguration.Instance.GetDockerHostOverride();

    /// <summary>
    /// Gets or sets the Docker socket override value.
    /// </summary>
    [CanBeNull]
    public static string DockerSocketOverride { get; set; }
      = DockerEndpointAuthProvider is ICustomConfiguration config
        ? config.GetDockerSocketOverride() : EnvironmentConfiguration.Instance.GetDockerSocketOverride() ?? PropertiesFileConfiguration.Instance.GetDockerSocketOverride();

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="ResourceReaper" /> is enabled or not.
    /// </summary>
    public static bool ResourceReaperEnabled { get; set; }
      = !EnvironmentConfiguration.Instance.GetRyukDisabled() && !PropertiesFileConfiguration.Instance.GetRyukDisabled();

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="ResourceReaper" /> privileged mode is enabled or not.
    /// </summary>
    public static bool ResourceReaperPrivilegedModeEnabled { get; set; }
      = EnvironmentConfiguration.Instance.GetRyukContainerPrivileged() || PropertiesFileConfiguration.Instance.GetRyukContainerPrivileged();

    /// <summary>
    /// Gets or sets the <see cref="ResourceReaper" /> image.
    /// </summary>
    [CanBeNull]
    public static IImage ResourceReaperImage { get; set; }
      = EnvironmentConfiguration.Instance.GetRyukContainerImage() ?? PropertiesFileConfiguration.Instance.GetRyukContainerImage();

    /// <summary>
    /// Gets or sets the <see cref="ResourceReaper" /> public host port.
    /// </summary>
    /// <remarks>
    /// The <see cref="ResourceReaper" /> might not be able to connect to Ryuk on Docker Desktop for Windows.
    /// Assigning a random port might run into the excluded port range. The container starts, but we cannot establish a TCP connection:
    /// - https://github.com/docker/for-win/issues/3171.
    /// - https://github.com/docker/for-win/issues/11584.
    /// </remarks>
    [NotNull]
    [Obsolete("The Resource Reaper will use Docker's assigned random host port. This property is no longer supported. For DinD configurations see: https://dotnet.testcontainers.org/examples/dind/.")]
    public static Func<IDockerEndpointAuthenticationConfiguration, ushort> ResourceReaperPublicHostPort { get; set; }
      = _ => 0;

    /// <summary>
    /// Gets or sets a prefix that applies to every image that is pulled from Docker Hub.
    /// </summary>
    /// <remarks>
    /// Please verify that all required images exist in your registry.
    /// </remarks>
    [CanBeNull]
    public static string HubImageNamePrefix { get; set; }
      = EnvironmentConfiguration.Instance.GetHubImageNamePrefix() ?? PropertiesFileConfiguration.Instance.GetHubImageNamePrefix();

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [NotNull]
    public static ILogger Logger { get; set; }
      = ConsoleLogger.Instance;

    /// <summary>
    /// Gets or sets the host operating system.
    /// </summary>
    [NotNull]
    public static IOperatingSystem OS { get; set; }
      = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? (IOperatingSystem)new Windows(DockerEndpointAuthConfig) : new Unix(DockerEndpointAuthConfig);

    /// <summary>
    /// Gets the wait handle that signals settings initialized.
    /// </summary>
    [NotNull]
    public static WaitHandle SettingsInitialized
      => ManualResetEvent.WaitHandle;

    /// <inheritdoc cref="PortForwardingContainer.ExposeHostPortsAsync" />
    public static Task ExposeHostPortsAsync(ushort port, CancellationToken ct = default)
      => ExposeHostPortsAsync(new[] { port }, ct);

    /// <inheritdoc cref="PortForwardingContainer.ExposeHostPortsAsync" />
    public static async Task ExposeHostPortsAsync(IEnumerable<ushort> ports, CancellationToken ct = default)
    {
      await PortForwardingContainer.Instance.StartAsync(ct)
        .ConfigureAwait(false);

      await PortForwardingContainer.Instance.ExposeHostPortsAsync(ports, ct)
        .ConfigureAwait(false);
    }
  }
}
