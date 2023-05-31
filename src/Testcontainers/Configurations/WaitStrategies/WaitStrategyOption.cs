namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;

  /// <inheritdoc cref="IWaitStrategyOption" />
  internal sealed class WaitStrategyOption : IWaitStrategyOption, IWaitUntil
  {
    private readonly IWaitUntil _waitUntil;

    /// <summary>
    /// Initializes a new instance of the <see cref="WaitStrategyOption" /> class.
    /// </summary>
    /// <param name="waitUntil">The wait strategy to run.</param>
    public WaitStrategyOption(IWaitUntil waitUntil)
    {
      _waitUntil = waitUntil;
      _ = WithRetries(1);
      _ = WithInterval(TimeSpan.FromSeconds(1));
      _ = WithInterval(TimeSpan.FromMinutes(1));
    }

    /// <inheritdoc />
    public ushort Retries { get; private set; }

    /// <inheritdoc />
    public TimeSpan Interval { get; private set; }

    /// <inheritdoc />
    public TimeSpan Timeout { get; private set; }

    /// <inheritdoc />
    public IWaitStrategyOption WithRetries(ushort retries)
    {
      Retries = retries;
      return this;
    }

    /// <inheritdoc />
    public IWaitStrategyOption WithInterval(TimeSpan interval)
    {
      Interval = interval;
      return this;
    }

    /// <inheritdoc />
    public IWaitStrategyOption WithTimeout(TimeSpan timeout)
    {
      Timeout = timeout;
      return this;
    }

    /// <inheritdoc />
    public Task<bool> UntilAsync(IContainer container)
    {
      return _waitUntil.UntilAsync(container);
    }
  }
}
