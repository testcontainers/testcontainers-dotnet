namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Net;
  using System.Net.Sockets;
  using System.Runtime.InteropServices;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Logging.Abstractions;

  /// <summary>
  /// This class represents the Testcontainers settings.
  /// </summary>
  [PublicAPI]
  public static class TestcontainersSettings
  {
    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="ResourceReaper" /> is enabled or not.
    /// </summary>
    [PublicAPI]
    public static bool ResourceReaperEnabled { get; set; }
      = true;

    /// <summary>
    /// Gets or sets the <see cref="ResourceReaper" /> image.
    /// </summary>
    [PublicAPI]
    public static IDockerImage ResourceReaperImage { get; set; }
      = new DockerImage("ghcr.io/psanetra/ryuk:2021.12.20");

    /// <summary>
    /// Gets or sets the <see cref="ResourceReaper" /> public host port.
    /// </summary>
    /// <remarks>
    /// The <see cref="ResourceReaper" /> might not be able to connect to Ryuk on Docker Desktop for Windows.
    /// Assigning a random port might run into the excluded port range. The container starts, but we can not establish a TCP connection:
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
      = string.Empty;

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [PublicAPI]
    [NotNull]
    public static ILogger Logger { get; set; }
      = NullLogger.Instance;

    /// <summary>
    /// Gets or sets the host operating system.
    /// </summary>
    [PublicAPI]
    [NotNull]
    public static IOperatingSystem OS { get; set; }
      = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? (IOperatingSystem)new Windows() : new Unix();

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
