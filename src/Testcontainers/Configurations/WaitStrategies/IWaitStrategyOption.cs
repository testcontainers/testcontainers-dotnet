namespace DotNet.Testcontainers.Configurations
{
  using System;

  /// <summary>
  ///
  /// </summary>
  public interface IWaitStrategyOption
  {
    /// <summary>
    ///
    /// </summary>
    ushort Retries { get; }

    /// <summary>
    ///
    /// </summary>
    TimeSpan Interval { get; }

    /// <summary>
    ///
    /// </summary>
    TimeSpan Timeout { get; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="retries"></param>
    /// <returns></returns>
    IWaitStrategyOption WithRetries(ushort retries);

    /// <summary>
    ///
    /// </summary>
    /// <param name="interval"></param>
    /// <returns></returns>
    IWaitStrategyOption WithInterval(TimeSpan interval);

    /// <summary>
    ///
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    IWaitStrategyOption WithTimeout(TimeSpan timeout);
  }
}
