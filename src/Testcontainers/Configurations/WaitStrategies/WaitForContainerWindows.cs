namespace DotNet.Testcontainers.Configurations
{
  using System;

  /// <inheritdoc cref="IWaitForContainerOS" />
  internal sealed class WaitForContainerWindows : WaitForContainerOS
  {
    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(string command, Action<IWaitStrategy> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(new UntilWindowsCommandIsCompleted(command), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(Action<IWaitStrategy> waitStrategyOptionModifier = null, params string[] command)
    {
      return AddCustomWaitStrategy(new UntilWindowsCommandIsCompleted(command), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilPortIsAvailable(int port, Action<IWaitStrategy> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(new UntilWindowsPortIsAvailable(port), waitStrategyOptionModifier);
    }
  }
}
