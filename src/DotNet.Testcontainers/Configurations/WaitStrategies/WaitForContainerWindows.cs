namespace DotNet.Testcontainers.Configurations
{
  /// <inheritdoc cref="IWaitForContainerOS" />
  internal sealed class WaitForContainerWindows : WaitForContainerOS
  {
    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(string command)
    {
      this.AddCustomWaitStrategy(new UntilWindowsCommandIsCompleted(command));
      return this;
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(params string[] command)
    {
      this.AddCustomWaitStrategy(new UntilWindowsCommandIsCompleted(command));
      return this;
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilPortIsAvailable(int port)
    {
      this.AddCustomWaitStrategy(new UntilWindowsPortIsAvailable(port));
      return this;
    }
  }
}
