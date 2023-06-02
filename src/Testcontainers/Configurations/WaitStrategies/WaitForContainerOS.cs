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
      AddCustomWaitStrategy(new UntilContainerIsRunning());
    }

    /// <inheritdoc />
    public abstract IWaitForContainerOS UntilCommandIsCompleted(string command, Action<IWaitStrategy> waitStrategyOptionModifier = null);

    /// <inheritdoc />
    public abstract IWaitForContainerOS UntilCommandIsCompleted(Action<IWaitStrategy> waitStrategyOptionModifier = null, params string[] command);

    /// <inheritdoc />
    public abstract IWaitForContainerOS UntilPortIsAvailable(int port, Action<IWaitStrategy> waitStrategyOptionModifier = null);

    /// <inheritdoc />
    public IWaitForContainerOS AddCustomWaitStrategy(IWaitUntil waitStrategy, Action<IWaitStrategy> waitStrategyOptionModifier = null)
    {
      var waitStrategyOption = new WaitStrategy(waitStrategy);

      if (waitStrategyOptionModifier != null)
      {
        waitStrategyOptionModifier(waitStrategyOption);
      }

      _waitStrategies.Add(waitStrategyOption);
      return this;
    }

    /// <inheritdoc />
    public IWaitForContainerOS UntilFileExists(string file, Action<IWaitStrategy> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(new UntilFilesExists(file), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public IWaitForContainerOS UntilMessageIsLogged(string pattern, Action<IWaitStrategy> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(new UntilMessageIsLogged(pattern), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public IWaitForContainerOS UntilMessageIsLogged(Regex pattern, Action<IWaitStrategy> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(new UntilMessageIsLogged(pattern), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public IWaitForContainerOS UntilMessageIsLogged(Stream stream, string message, Action<IWaitStrategy> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(new UntilMessageIsLogged(message), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public IWaitForContainerOS UntilOperationIsSucceeded(Func<bool> operation, int maxCallCount, Action<IWaitStrategy> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(new UntilOperationIsSucceeded(operation, maxCallCount), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public IWaitForContainerOS UntilHttpRequestIsSucceeded(Func<HttpWaitStrategy, HttpWaitStrategy> request, Action<IWaitStrategy> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(request.Invoke(new HttpWaitStrategy()), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public IWaitForContainerOS UntilContainerIsHealthy(long failingStreak = 3, Action<IWaitStrategy> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(new UntilContainerIsHealthy(failingStreak), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public IEnumerable<WaitStrategy> Build()
    {
      return _waitStrategies;
    }
  }
}
