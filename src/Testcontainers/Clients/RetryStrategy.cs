namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// A retry strategy that executes an action until it completes successfully or the maximum number of attempts is exceeded.
  /// </summary>
  internal class RetryStrategy
  {
    private const double JitterFactor = 0.25;

#if !NET6_0_OR_GREATER
    private static readonly Random Random = new Random();
#endif

    private Func<Exception, bool> _retryOn = _ => false;

    private Action<int, TimeSpan, Exception> _onRetry = (_, _, _) => { };

    /// <summary>
    /// Gets the maximum number of attempts.
    /// </summary>
    public int MaxAttempts { get; private set; }
      = 1;

    /// <summary>
    /// Gets the interval between retries.
    /// </summary>
    public TimeSpan Interval { get; private set; }
      = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Sets the maximum number of attempts.
    /// </summary>
    /// <param name="maxAttempts">The maximum number of attempts, including the initial one.</param>
    /// <returns>The updated instance of the retry strategy.</returns>
    public RetryStrategy WithMaxAttempts(int maxAttempts)
    {
      MaxAttempts = maxAttempts;
      return this;
    }

    /// <summary>
    /// Sets the interval between retries.
    /// </summary>
    /// <param name="interval">The interval the strategy waits before the second attempt.</param>
    /// <returns>The updated instance of the retry strategy.</returns>
    public RetryStrategy WithInterval(TimeSpan interval)
    {
      Interval = interval;
      return this;
    }

    /// <summary>
    /// Sets the condition that determines whether a failed attempt is retried.
    /// </summary>
    /// <param name="retryOn">A function that returns true if the exception is retryable; otherwise, false.</param>
    /// <returns>The updated instance of the retry strategy.</returns>
    public RetryStrategy WithRetryOn(Func<Exception, bool> retryOn)
    {
      _retryOn = retryOn;
      return this;
    }

    /// <summary>
    /// Sets the callback that is invoked before the strategy waits for the next attempt.
    /// </summary>
    /// <param name="onRetry">An action that receives the upcoming attempt, the delay before it and the exception that caused the retry.</param>
    /// <returns>The updated instance of the retry strategy.</returns>
    public RetryStrategy WithOnRetry(Action<int, TimeSpan, Exception> onRetry)
    {
      _onRetry = onRetry;
      return this;
    }

    /// <summary>
    /// Executes the action until it completes successfully or the maximum number of attempts is exceeded.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous execute operation.</returns>
    public async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken ct = default)
    {
      for (var attempt = 1; ; attempt++)
      {
        ct.ThrowIfCancellationRequested();

        try
        {
          await action(ct)
            .ConfigureAwait(false);

          return;
        }
        catch (Exception e) when (attempt < MaxAttempts && !ct.IsCancellationRequested && _retryOn(e))
        {
          var delay = GetDelay(attempt);

          _onRetry(attempt + 1, delay, e);

          await DelayAsync(delay, ct)
            .ConfigureAwait(false);
        }
      }
    }

    /// <summary>
    /// Gets the delay before the next attempt, applying an exponential backoff with jitter.
    /// </summary>
    /// <param name="attempt">The attempt that just failed.</param>
    /// <returns>The delay before the next attempt.</returns>
    protected virtual TimeSpan GetDelay(int attempt)
    {
      var backoff = Interval.TotalMilliseconds * Math.Pow(2, attempt - 1);

#if NET6_0_OR_GREATER
      var jitter = JitterFactor * backoff * Random.Shared.NextDouble();
#else
      double jitter;

      // Random.Shared requires .NET 6 or greater. A shared Random instance is not thread-safe.
      lock (Random)
      {
        jitter = JitterFactor * backoff * Random.NextDouble();
      }
#endif

      return TimeSpan.FromMilliseconds(backoff + jitter);
    }

    /// <summary>
    /// Waits the specified delay before the next attempt.
    /// </summary>
    /// <param name="delay">The delay before the next attempt.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous delay operation.</returns>
    protected virtual Task DelayAsync(TimeSpan delay, CancellationToken ct)
    {
      return Task.Delay(delay, ct);
    }
  }
}
