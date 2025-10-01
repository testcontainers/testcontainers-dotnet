namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  /// <inheritdoc cref="IWaitForContainerOS" />
  internal sealed class WaitForContainerWindows : WaitForContainerOS
  {
    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(params string[] command)
    {
      return AddCustomWaitStrategy(new UntilWindowsCommandIsCompleted(command));
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(string command, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      return AddCustomWaitStrategy(new UntilWindowsCommandIsCompleted(command), waitStrategyModifier);
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(IEnumerable<string> command, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      return AddCustomWaitStrategy(new UntilWindowsCommandIsCompleted(command.ToArray()), waitStrategyModifier);
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilInternalTcpPortIsAvailable(int containerPort, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      return AddCustomWaitStrategy(new UntilInternalTcpPortIsAvailableOnWindows(containerPort), waitStrategyModifier);
    }
  }
}
