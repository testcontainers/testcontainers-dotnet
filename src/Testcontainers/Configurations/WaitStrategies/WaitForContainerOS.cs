namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Data.Common;
  using System.Text.RegularExpressions;

  /// <inheritdoc cref="IWaitForContainerOS" />
  internal abstract class WaitForContainerOS : IWaitForContainerOS
  {
    private readonly ICollection<WaitStrategy> _waitStrategies = new List<WaitStrategy>();

    /// <summary>
    /// Initializes a new instance of the <see cref="WaitForContainerOS" /> class.
    /// </summary>
    protected WaitForContainerOS()
    {
      _waitStrategies.Add(new WaitStrategy(new UntilContainerIsRunning()));
    }

    /// <inheritdoc />
    public abstract IWaitForContainerOS UntilCommandIsCompleted(params string[] command);

    /// <inheritdoc />
    public abstract IWaitForContainerOS UntilCommandIsCompleted(string command, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <inheritdoc />
    public abstract IWaitForContainerOS UntilCommandIsCompleted(IEnumerable<string> command, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <inheritdoc />
    public abstract IWaitForContainerOS UntilPortIsAvailable(int port, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <inheritdoc />
    public virtual IWaitForContainerOS AddCustomWaitStrategy(IWaitUntil waitUntil, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      var waitStrategy = new WaitStrategy(waitUntil);

      if (waitStrategyModifier != null)
      {
        waitStrategyModifier(waitStrategy);
      }

      _waitStrategies.Add(waitStrategy);
      return this;
    }

    /// <inheritdoc />
    public virtual IWaitForContainerOS UntilFileExists(string filePath, FileSystem fileSystem = FileSystem.Host, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      switch (fileSystem)
      {
        case FileSystem.Container:
          return AddCustomWaitStrategy(new UntilFileExistsInContainer(filePath), waitStrategyModifier);
        case FileSystem.Host:
        default:
          return AddCustomWaitStrategy(new UntilFileExistsOnHost(filePath), waitStrategyModifier);
      }
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
    public virtual IWaitForContainerOS UntilTcpConnected(int port, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      return AddCustomWaitStrategy(new UntilTcpConnected(port), waitStrategyModifier);
    }

    /// <inheritdoc />
    public virtual IWaitForContainerOS UntilHttpRequestIsSucceeded(Func<HttpWaitStrategy, HttpWaitStrategy> request, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      return AddCustomWaitStrategy(request.Invoke(new HttpWaitStrategy()), waitStrategyModifier);
    }

    /// <inheritdoc />
    public virtual IWaitForContainerOS UntilContainerIsHealthy(long failingStreak = 3, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      return AddCustomWaitStrategy(new UntilContainerIsHealthy(failingStreak), waitStrategyModifier);
    }

    /// <inheritdoc />
    public virtual IWaitForContainerOS UntilDatabaseIsAvailable(DbProviderFactory dbProviderFactory, Action<IWaitStrategy> waitStrategyModifier = null)
    {
      return AddCustomWaitStrategy(new UntilDatabaseIsAvailable(dbProviderFactory), waitStrategyModifier);
    }

    /// <inheritdoc />
    public IEnumerable<WaitStrategy> Build()
    {
      return _waitStrategies;
    }
  }
}
