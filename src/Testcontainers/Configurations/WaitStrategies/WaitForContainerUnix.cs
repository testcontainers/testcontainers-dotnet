namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  /// <inheritdoc cref="IWaitForContainerOS" />
  internal sealed class WaitForContainerUnix : WaitForContainerOS
  {
    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(params string[] command)
    {
      return AddCustomWaitStrategy(new UntilUnixCommandIsCompleted(command));
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(string command, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      return AddCustomWaitStrategy(new UntilUnixCommandIsCompleted(command), waitStrategyModifier);
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(IEnumerable<string> command, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      return AddCustomWaitStrategy(new UntilUnixCommandIsCompleted(command.ToArray()), waitStrategyModifier);
    }

    public override IWaitForContainerOS UntilHostPortAvailable(int port, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      UntilPortIsAvailable(port, waitStrategyModifier);
      return AddCustomWaitStrategy(new HostPortWaitStrategy(port), waitStrategyModifier);
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilPortIsAvailable(int port, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      return AddCustomWaitStrategy(new UntilUnixPortIsAvailable(port), waitStrategyModifier);
    }
  }
}
