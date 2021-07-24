namespace DotNet.Testcontainers.Configurations
{
  using System.Runtime.InteropServices;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Logging.Abstractions;

  /// <summary>
  /// This class represents the Testcontainers settings.
  /// </summary>
  public static class TestcontainersSettings
  {
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
      = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? (IOperatingSystem)default(Windows) : default(Unix);
  }
}
