namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IWaitStrategy" />
  [PublicAPI]
  public class WaitStrategy : IWaitStrategy
  {
    private const string MaximumRetryExceededException = "The maximum number of retries has been exceeded.";

    private IWaitWhile _waitWhile;

    private IWaitUntil _waitUntil;

    /// <summary>
    /// Initializes a new instance of the <see cref="WaitStrategy" /> class.
    /// </summary>
    public WaitStrategy()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WaitStrategy" /> class.
    /// </summary>
    /// <param name="waitWhile">The wait while condition to be used in the strategy.</param>
    public WaitStrategy(IWaitWhile waitWhile)
    {
      _ = WithStrategy(waitWhile);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WaitStrategy" /> class.
    /// </summary>
    /// <param name="waitUntil">The wait until condition to be used in the strategy.</param>
    public WaitStrategy(IWaitUntil waitUntil)
    {
      _ = WithStrategy(waitUntil);
    }

    /// <summary>
    /// Gets the wait strategy mode.
    /// </summary>
    public WaitStrategyMode Mode { get; private set; }
      =  WaitStrategyMode.Running;

    /// <summary>
    /// Gets the number of retries.
    /// </summary>
    public ushort Retries { get; private set; }
      = TestcontainersSettings.WaitStrategyRetries ?? 0;

    /// <summary>
    /// Gets the interval between retries.
    /// </summary>
    public TimeSpan Interval { get; private set; }
      = TestcontainersSettings.WaitStrategyInterval ?? TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets the timeout.
    /// </summary>
    public TimeSpan Timeout { get; private set; }
      = TestcontainersSettings.WaitStrategyTimeout ?? TimeSpan.FromHours(1);

    /// <inheritdoc />
    public IWaitStrategy WithMode(WaitStrategyMode mode)
    {
      Mode = mode;
      return this;
    }

    /// <inheritdoc />
    public IWaitStrategy WithRetries(ushort retries)
    {
      Retries = retries;
      return this;
    }

    /// <inheritdoc />
    public IWaitStrategy WithInterval(TimeSpan interval)
    {
      Interval = interval;
      return this;
    }

    /// <inheritdoc />
    public IWaitStrategy WithTimeout(TimeSpan timeout)
    {
      Timeout = timeout;
      return this;
    }

    /// <summary>
    /// Executes the wait strategy while the container satisfies the condition.
    /// </summary>
    /// <param name="container">The container to check the condition for.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, returning false if the container satisfies the condition; otherwise, true.</returns>
    public virtual Task<bool> WhileAsync(IContainer container, CancellationToken ct = default)
    {
      return _waitWhile.WhileAsync(container);
    }

    /// <summary>
    /// Executes the wait strategy until the container satisfies the condition.
    /// </summary>
    /// <param name="container">The container to check the condition for.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, returning true if the container satisfies the condition; otherwise, false.</returns>
    public virtual Task<bool> UntilAsync(IContainer container, CancellationToken ct = default)
    {
      return _waitUntil.UntilAsync(container);
    }

    /// <summary>
    /// Waits asynchronously until the specified condition returns false or until a timeout occurs.
    /// </summary>
    /// <remarks>
    /// Zero or a negative value for <paramref name="retries" /> will run the readiness check infinitely until it becomes false.
    /// </remarks>
    /// <param name="wait">A function that represents the asynchronous condition to wait for.</param>
    /// <param name="interval">The time interval between consecutive evaluations of the condition.</param>
    /// <param name="timeout">The maximum duration to wait for the condition to become false.</param>
    /// <param name="retries">The number of retries to run for the condition to become false.</param>
    /// <param name="ct">The optional cancellation token to cancel the waiting operation.</param>
    /// <exception cref="TimeoutException">Thrown when the timeout expires.</exception>
    /// <exception cref="RetryLimitExceededException">Thrown when the number of retries is exceeded.</exception>
    /// <returns>A task that represents the asynchronous block operation.</returns>
    [PublicAPI]
    public static Task WaitWhileAsync(Func<Task<bool>> wait, TimeSpan interval, TimeSpan timeout, int retries = 0, CancellationToken ct = default)
    {
      return WaitAsync(wait, false, interval, timeout, retries, ct);
    }

    /// <summary>
    /// Waits asynchronously until the specified condition returns true or until a timeout occurs.
    /// </summary>
    /// <remarks>
    /// Zero or a negative value for <paramref name="retries" /> will run the readiness check infinitely until it becomes true.
    /// </remarks>
    /// <param name="wait">A function that represents the asynchronous condition to wait for.</param>
    /// <param name="interval">The time interval between consecutive evaluations of the condition.</param>
    /// <param name="timeout">The maximum duration to wait for the condition to become true.</param>
    /// <param name="retries">The number of retries to run for the condition to become true.</param>
    /// <param name="ct">The optional cancellation token to cancel the waiting operation.</param>
    /// <exception cref="TimeoutException">Thrown when the timeout expires.</exception>
    /// <exception cref="RetryLimitExceededException">Thrown when the number of retries is exceeded.</exception>
    /// <returns>A task that represents the asynchronous block operation.</returns>
    [PublicAPI]
    public static Task WaitUntilAsync(Func<Task<bool>> wait, TimeSpan interval, TimeSpan timeout, int retries = 0, CancellationToken ct = default)
    {
      return WaitAsync(wait, true, interval, timeout, retries, ct);
    }

    /// <summary>
    /// Waits asynchronously until the specified condition returns the expected result or until a timeout occurs.
    /// </summary>
    /// <param name="wait">A function that represents the asynchronous condition to wait for.</param>
    /// <param name="expected">The result of the condition that completes the waiting operation.</param>
    /// <param name="interval">The time interval between consecutive evaluations of the condition.</param>
    /// <param name="timeout">The maximum duration to wait for the condition to return the expected result.</param>
    /// <param name="retries">The number of retries to run for the condition to return the expected result.</param>
    /// <param name="ct">The cancellation token to cancel the waiting operation.</param>
    /// <exception cref="TimeoutException">Thrown when the timeout expires.</exception>
    /// <exception cref="RetryLimitExceededException">Thrown when the number of retries is exceeded.</exception>
    /// <returns>A task that represents the asynchronous block operation.</returns>
    private static async Task WaitAsync(Func<Task<bool>> wait, bool expected, TimeSpan interval, TimeSpan timeout, int retries, CancellationToken ct)
    {
      using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct))
      {
        try
        {
          var waitTask = EvaluateAsync(linkedCts.Token);

          var timeoutTask = Task.Delay(timeout, linkedCts.Token);

          var isTimeoutTask = timeoutTask == await Task.WhenAny(waitTask, timeoutTask)
            .ConfigureAwait(false);

          if (isTimeoutTask)
          {
            // The timeout task completes when the waiting operation is canceled too.
            ct.ThrowIfCancellationRequested();
            throw new TimeoutException();
          }

          // Rethrows exceptions.
          await waitTask
            .ConfigureAwait(false);
        }
        finally
        {
          // Stop the task that is still pending. Either the timeout, or the
          // evaluation of the condition that did not complete in time.
          linkedCts.Cancel();
        }
      }

      async Task EvaluateAsync(CancellationToken linkedCt)
      {
        var actualRetries = 0;

        while (true)
        {
          linkedCt.ThrowIfCancellationRequested();

          var actual = await wait.Invoke()
            .ConfigureAwait(false);

          if (expected.Equals(actual))
          {
            break;
          }

          if (retries > 0 && ++actualRetries > retries)
          {
            throw new RetryLimitExceededException(MaximumRetryExceededException);
          }

          await Task.Delay(interval, linkedCt)
            .ConfigureAwait(false);
        }
      }
    }

    /// <summary>
    /// Sets the wait while condition.
    /// </summary>
    /// <param name="waitWhile">The wait while condition to be used in the strategy.</param>
    /// <returns>The updated instance of the wait strategy.</returns>
    private WaitStrategy WithStrategy(IWaitWhile waitWhile)
    {
      _waitWhile = waitWhile;
      return this;
    }

    /// <summary>
    /// Sets the wait until condition.
    /// </summary>
    /// <param name="waitUntil">The wait until condition to be used in the strategy.</param>
    /// <returns>The updated instance of the wait strategy.</returns>
    private WaitStrategy WithStrategy(IWaitUntil waitUntil)
    {
      _waitUntil = waitUntil;
      return this;
    }
  }
}
