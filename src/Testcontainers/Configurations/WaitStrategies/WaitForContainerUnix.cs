namespace DotNet.Testcontainers.Configurations
{
  using System;

  /// <inheritdoc cref="IWaitForContainerOS" />
  internal sealed class WaitForContainerUnix : WaitForContainerOS
  {
    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(string command, Action<IWaitStrategy> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(new UntilUnixCommandIsCompleted(command), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(Action<IWaitStrategy> waitStrategyOptionModifier = null, params string[] command)
    {
      return AddCustomWaitStrategy(new UntilUnixCommandIsCompleted(command), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilPortIsAvailable(int port, Action<IWaitStrategy> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(new UntilUnixPortIsAvailable(port), waitStrategyOptionModifier);
    }
  }
}
