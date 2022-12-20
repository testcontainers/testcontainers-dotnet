namespace DotNet.Testcontainers.Configurations
{
  /// <inheritdoc cref="IWaitForContainerOS" />
  internal sealed class WaitForContainerWindows : WaitForContainerOS
  {
    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(string command)
    {
      return this.AddCustomWaitStrategy(new UntilWindowsCommandIsCompleted(command));
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(params string[] command)
    {
      return this.AddCustomWaitStrategy(new UntilWindowsCommandIsCompleted(command));
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilPortIsAvailable(int port)
    {
      return this.AddCustomWaitStrategy(new UntilWindowsPortIsAvailable(port));
    }
  }
}
