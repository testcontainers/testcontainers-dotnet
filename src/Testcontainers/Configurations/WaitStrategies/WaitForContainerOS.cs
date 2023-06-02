namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Text.RegularExpressions;

  /// <inheritdoc cref="IWaitForContainerOS" />
  internal abstract class WaitForContainerOS : IWaitForContainerOS
  {
    private readonly IList<WaitStrategy> _waitStrategies = new List<WaitStrategy>();

    /// <summary>
    /// Initializes a new instance of the <see cref="WaitForContainerOS" /> class.
    /// </summary>
    protected WaitForContainerOS()
    {
      _ = AddCustomWaitStrategy(new UntilContainerIsRunning());
    }

    /// <inheritdoc />
    public abstract IWaitForContainerOS UntilCommandIsCompleted(string command, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <inheritdoc />
    public abstract IWaitForContainerOS UntilCommandIsCompleted(Action<IWaitStrategy> waitStrategyModifier = null, params string[] command);

    /// <inheritdoc />
    public abstract IWaitForContainerOS UntilPortIsAvailable(int port, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <inheritdoc />
    public IWaitForContainerOS AddCustomWaitStrategy(IWaitUntil waitUntil, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      var waitStrategy = new WaitStrategy();
      _ = waitStrategy.WithStrategy(waitUntil);

      if (waitStrategyModifier != null)
      {
        waitStrategyModifier(waitStrategy);
      }

      _waitStrategies.Add(waitStrategy);
      return this;
    }

    /// <inheritdoc />
    public IWaitForContainerOS UntilFileExists(string file, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      return AddCustomWaitStrategy(new UntilFilesExists(file), waitStrategyModifier);
    }

    /// <inheritdoc />
    public IWaitForContainerOS UntilMessageIsLogged(string pattern, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      return AddCustomWaitStrategy(new UntilMessageIsLogged(pattern), waitStrategyModifier);
    }

    /// <inheritdoc />
    public IWaitForContainerOS UntilMessageIsLogged(Regex pattern, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      return AddCustomWaitStrategy(new UntilMessageIsLogged(pattern), waitStrategyModifier);
    }

    /// <inheritdoc />
    public IWaitForContainerOS UntilMessageIsLogged(Stream stream, string message, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      return AddCustomWaitStrategy(new UntilMessageIsLogged(message), waitStrategyModifier);
    }

    /// <inheritdoc />
    public IWaitForContainerOS UntilOperationIsSucceeded(Func<bool> operation, int maxCallCount, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      return AddCustomWaitStrategy(new UntilOperationIsSucceeded(operation, maxCallCount), waitStrategyModifier);
    }

    /// <inheritdoc />
    public IWaitForContainerOS UntilHttpRequestIsSucceeded(Func<HttpWaitStrategy, HttpWaitStrategy> request, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      return AddCustomWaitStrategy(request.Invoke(new HttpWaitStrategy()), waitStrategyModifier);
    }

    /// <inheritdoc />
    public IWaitForContainerOS UntilContainerIsHealthy(long failingStreak = 3, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      return AddCustomWaitStrategy(new UntilContainerIsHealthy(failingStreak), waitStrategyModifier);
    }

    /// <inheritdoc />
    public IEnumerable<WaitStrategy> Build()
    {
      return _waitStrategies;
    }
  }
}
