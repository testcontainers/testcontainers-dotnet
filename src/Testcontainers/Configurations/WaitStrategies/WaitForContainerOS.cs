namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.IO;

  /// <inheritdoc cref="IWaitForContainerOS" />
  internal abstract class WaitForContainerOS : IWaitForContainerOS
  {
    private readonly ICollection<IWaitUntil> waitStrategies = new List<IWaitUntil>();

    protected WaitForContainerOS()
    {
      this.waitStrategies.Add(new UntilContainerIsRunning());
    }

    /// <inheritdoc />
    public abstract IWaitForContainerOS UntilCommandIsCompleted(string command);

    /// <inheritdoc />
    public abstract IWaitForContainerOS UntilCommandIsCompleted(params string[] command);

    /// <inheritdoc />
    public abstract IWaitForContainerOS UntilPortIsAvailable(int port);

    /// <inheritdoc />
    public virtual IWaitForContainerOS AddCustomWaitStrategy(IWaitUntil waitStrategy)
    {
      this.waitStrategies.Add(waitStrategy);
      return this;
    }

    /// <inheritdoc />
    public virtual IWaitForContainerOS UntilFileExists(string file)
    {
      return this.AddCustomWaitStrategy(new UntilFilesExists(file));
    }

    /// <inheritdoc />
    public virtual IWaitForContainerOS UntilMessageIsLogged(Stream stream, string message)
    {
      return this.AddCustomWaitStrategy(new UntilMessageIsLogged(stream, message));
    }

    /// <inheritdoc />
    public virtual IWaitForContainerOS UntilOperationIsSucceeded(Func<bool> operation, int maxCallCount)
    {
      return this.AddCustomWaitStrategy(new UntilOperationIsSucceeded(operation, maxCallCount));
    }

    /// <inheritdoc />
    public IEnumerable<IWaitUntil> Build()
    {
      return this.waitStrategies;
    }
  }
}
