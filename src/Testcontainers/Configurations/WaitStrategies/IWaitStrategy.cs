namespace DotNet.Testcontainers.Configurations
{
  using System;

  /// <summary>
  /// Represents a wait strategy configuration.
  /// </summary>
  public interface IWaitStrategy
  {
    /// <summary>
    /// Sets the number of retries for the wait strategy.
    /// </summary>
    /// <param name="retries">The number of retries.</param>
    /// <returns>The updated instance of the wait strategy.</returns>
    IWaitStrategy WithRetries(ushort retries);

    /// <summary>
    /// Sets the interval between retries for the wait strategy.
    /// </summary>
    /// <param name="interval">The interval between retries.</param>
    /// <returns>The updated instance of the wait strategy.</returns>
    IWaitStrategy WithInterval(TimeSpan interval);

    /// <summary>
    /// Sets the timeout for the wait strategy.
    /// </summary>
    /// <param name="timeout">The timeout duration.</param>
    /// <returns>The updated instance of the wait strategy.</returns>
    IWaitStrategy WithTimeout(TimeSpan timeout);
  }
}
