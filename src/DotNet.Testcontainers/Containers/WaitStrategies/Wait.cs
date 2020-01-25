namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  /// <summary>
  /// Container platform specific wait strategies.
  /// </summary>
  public static class Wait
  {
    /// <summary>
    /// Returns the pre-configured wait strategies for Unix containers.
    /// </summary>
    /// <returns>Implementation of <see cref="IWaitForContainerOS" /> for Unix containers.</returns>
    public static IWaitForContainerOS ForUnixContainer()
    {
      return new WaitForContainerUnix();
    }

    /// <summary>
    /// Returns the pre-configured wait strategies for Windows containers.
    /// </summary>
    /// <returns>Implementation of <see cref="IWaitForContainerOS" /> for Windows containers.</returns>
    public static IWaitForContainerOS ForWindowsContainer()
    {
      return new WaitForContainerWindows();
    }
  }
}
