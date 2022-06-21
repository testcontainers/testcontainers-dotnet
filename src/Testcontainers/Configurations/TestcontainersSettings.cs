namespace DotNet.Testcontainers.Configurations
{
  using System.Runtime.InteropServices;
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
    /// Gets or sets a docker image for <see cref="ResourceReaper" /> container.
    /// </summary>
    [PublicAPI]
    public static IDockerImage ResourceReaperImage { get; set; }
      = new DockerImage("ghcr.io/psanetra/ryuk:2021.12.20");

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
  }
}
