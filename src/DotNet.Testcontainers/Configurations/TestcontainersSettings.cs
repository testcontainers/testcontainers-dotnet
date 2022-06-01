namespace DotNet.Testcontainers.Configurations
{
  using System.Runtime.InteropServices;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Logging.Abstractions;

  /// <summary>
  /// This class represents the Testcontainers settings.
  /// </summary>
  public static class TestcontainersSettings
  {
    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="ResourceReaper" /> is enabled or not.
    /// </summary>
    public static bool ResourceReaperEnabled { get; set; }
      = true;

    /// <summary>
    /// Gets or sets a prefix to prepend to image names for images hosted on DockerHub.
    /// </summary>
    public static string DockerHubImagePrefix { get; set; }

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [NotNull]
    public static ILogger Logger { get; set; }
      = NullLogger.Instance;

    /// <summary>
    /// Gets or sets the host operating system.
    /// </summary>
    [NotNull]
    public static IOperatingSystem OS { get; set; }
      = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? (IOperatingSystem)new Windows() : new Unix();
  }
}
