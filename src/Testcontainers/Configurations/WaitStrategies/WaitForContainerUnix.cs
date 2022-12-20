namespace DotNet.Testcontainers.Configurations
{
  /// <inheritdoc cref="IWaitForContainerOS" />
  internal sealed class WaitForContainerUnix : WaitForContainerOS
  {
    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(string command)
    {
      return this.AddCustomWaitStrategy(new UntilUnixCommandIsCompleted(command));
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(params string[] command)
    {
      return this.AddCustomWaitStrategy(new UntilUnixCommandIsCompleted(command));
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilPortIsAvailable(int port)
    {
      return this.AddCustomWaitStrategy(new UntilUnixPortIsAvailable(port));
    }
  }
}
