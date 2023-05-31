namespace DotNet.Testcontainers.Configurations
{
  using System;

  /// <summary>
  /// Represents options for a wait strategy.
  /// </summary>
  public interface IWaitStrategyOption
  {
    /// <summary>
    /// Gets the maximum number of retries to wait for the condition.
    /// </summary>
    ushort Retries { get; }

    /// <summary>
    /// Gets the time interval between consecutive evaluations of the condition.
    /// </summary>
    TimeSpan Interval { get; }

    /// <summary>
    /// Gets the maximum duration to wait for the condition.
    /// </summary>
    TimeSpan Timeout { get; }

    /// <summary>
    /// Specifies the maximum number of retries to wait for the condition.
    /// </summary>
    /// <param name="retries">The maximum number of retries.</param>
    /// <returns>A configured instance of <see cref="IWaitStrategyOption" />.</returns>
    IWaitStrategyOption WithRetries(ushort retries);

    /// <summary>
    /// Specifies the time interval between consecutive evaluations of the condition.
    /// </summary>
    /// <param name="interval">The time interval between evaluations.</param>
    /// <returns>A configured instance of <see cref="IWaitStrategyOption" />.</returns>
    IWaitStrategyOption WithInterval(TimeSpan interval);

    /// <summary>
    /// Specifies the maximum duration to wait for the condition.
    /// </summary>
    /// <param name="timeout">The maximum duration to wait.</param>
    /// <returns>A configured instance of <see cref="IWaitStrategyOption" />.</returns>
    IWaitStrategyOption WithTimeout(TimeSpan timeout);
  }
}
