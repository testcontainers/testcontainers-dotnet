namespace DotNet.Testcontainers.Builders
{
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <summary>
  /// Container platform specific wait strategies.
  /// </summary>
  [PublicAPI]
  public static class Wait
  {
    /// <summary>
    /// Returns the pre-configured wait strategies for Unix containers.
    /// </summary>
    /// <returns>Implementation of <see cref="IWaitForContainerOS" /> for Unix containers.</returns>
    [PublicAPI]
    public static IWaitForContainerOS ForUnixContainer()
    {
      return new WaitForContainerUnix();
    }

    /// <summary>
    /// Returns the pre-configured wait strategies for Windows containers.
    /// </summary>
    /// <returns>Implementation of <see cref="IWaitForContainerOS" /> for Windows containers.</returns>
    [PublicAPI]
    public static IWaitForContainerOS ForWindowsContainer()
    {
      return new WaitForContainerWindows();
    }
  }
}
