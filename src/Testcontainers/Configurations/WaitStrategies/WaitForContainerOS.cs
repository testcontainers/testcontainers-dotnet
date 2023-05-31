namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Text.RegularExpressions;

  /// <inheritdoc cref="IWaitForContainerOS" />
  internal abstract class WaitForContainerOS : IWaitForContainerOS
  {
    private readonly IList<IWaitUntil> _waitStrategies = new List<IWaitUntil>();

    /// <summary>
    /// Initializes a new instance of the <see cref="WaitForContainerOS" /> class.
    /// </summary>
    protected WaitForContainerOS()
    {
      _waitStrategies.Add(new UntilContainerIsRunning());
    }

    /// <inheritdoc />
    public abstract IWaitForContainerOS UntilCommandIsCompleted(string command, Action<IWaitStrategyOption> waitStrategyOptionModifier = null);

    /// <inheritdoc />
    public abstract IWaitForContainerOS UntilCommandIsCompleted(Action<IWaitStrategyOption> waitStrategyOptionModifier = null, params string[] command);

    /// <inheritdoc />
    public abstract IWaitForContainerOS UntilPortIsAvailable(int port, Action<IWaitStrategyOption> waitStrategyOptionModifier = null);

    /// <inheritdoc />
    public virtual IWaitForContainerOS AddCustomWaitStrategy(IWaitUntil waitStrategy, Action<IWaitStrategyOption> waitStrategyOptionModifier = null)
    {
      var waitStrategyOption = new WaitStrategyOption(waitStrategy);

      if (waitStrategyOptionModifier != null)
      {
        waitStrategyOptionModifier(waitStrategyOption);
      }

      _waitStrategies.Add(waitStrategyOption);
      return this;
    }

    /// <inheritdoc />
    public virtual IWaitForContainerOS UntilFileExists(string file, Action<IWaitStrategyOption> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(new UntilFilesExists(file), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public IWaitForContainerOS UntilMessageIsLogged(string pattern, Action<IWaitStrategyOption> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(new UntilMessageIsLogged(pattern), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public IWaitForContainerOS UntilMessageIsLogged(Regex pattern, Action<IWaitStrategyOption> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(new UntilMessageIsLogged(pattern), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public virtual IWaitForContainerOS UntilMessageIsLogged(Stream stream, string message, Action<IWaitStrategyOption> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(new UntilMessageIsLogged(message), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public virtual IWaitForContainerOS UntilOperationIsSucceeded(Func<bool> operation, int maxCallCount, Action<IWaitStrategyOption> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(new UntilOperationIsSucceeded(operation, maxCallCount), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public virtual IWaitForContainerOS UntilHttpRequestIsSucceeded(Func<HttpWaitStrategy, HttpWaitStrategy> request, Action<IWaitStrategyOption> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(request.Invoke(new HttpWaitStrategy()), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public virtual IWaitForContainerOS UntilContainerIsHealthy(long failingStreak = 3, Action<IWaitStrategyOption> waitStrategyOptionModifier = null)
    {
      return AddCustomWaitStrategy(new UntilContainerIsHealthy(failingStreak), waitStrategyOptionModifier);
    }

    /// <inheritdoc />
    public IEnumerable<IWaitUntil> Build()
    {
      return _waitStrategies;
    }
  }
}
