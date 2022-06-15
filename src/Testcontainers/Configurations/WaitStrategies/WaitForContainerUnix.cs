namespace DotNet.Testcontainers.Configurations
{
  /// <inheritdoc cref="IWaitForContainerOS" />
  internal sealed class WaitForContainerUnix : WaitForContainerOS
  {
    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(string command)
    {
      this.AddCustomWaitStrategy(new UntilUnixCommandIsCompleted(command));
      return this;
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(params string[] command)
    {
      this.AddCustomWaitStrategy(new UntilUnixCommandIsCompleted(command));
      return this;
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilPortIsAvailable(int port)
    {
      this.AddCustomWaitStrategy(new UntilUnixPortIsAvailable(port));
      return this;
    }
  }
}
