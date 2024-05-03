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
    /// Gets the number of retries.
    /// </summary>
    public ushort Retries { get; private set; }
      = ushort.MinValue;

    /// <summary>
    /// Gets the interval between retries.
    /// </summary>
    public TimeSpan Interval { get; private set; }
      = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets the timeout.
    /// </summary>
    public TimeSpan Timeout { get; private set; }
      = TimeSpan.FromHours(1);

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
    /// <exception cref="ArgumentException">Thrown when the number of retries is exceeded.</exception>
    /// <returns>A task that represents the asynchronous block operation.</returns>
    [PublicAPI]
    public static async Task WaitWhileAsync(Func<Task<bool>> wait, TimeSpan interval, TimeSpan timeout, int retries = -1, CancellationToken ct = default)
    {
      ushort actualRetries = 0;

      async Task WhileAsync()
      {
        while (!ct.IsCancellationRequested)
        {
          var isSuccessful = await wait.Invoke()
            .ConfigureAwait(false);

          if (!isSuccessful)
          {
            break;
          }

          _ = Guard.Argument(retries, nameof(retries))
            .ThrowIf(_ => retries > 0 && ++actualRetries > retries, _ => throw new ArgumentException(MaximumRetryExceededException));

          await Task.Delay(interval, ct)
            .ConfigureAwait(false);
        }
      }

      var waitTask = WhileAsync();

      var timeoutTask = Task.Delay(timeout, ct);

      var isTimeoutTask = timeoutTask == await Task.WhenAny(waitTask, timeoutTask)
        .ConfigureAwait(false);

      if (isTimeoutTask)
      {
        throw new TimeoutException();
      }

      // Rethrows exceptions.
      await waitTask
        .ConfigureAwait(false);
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
    /// <exception cref="ArgumentException">Thrown when the number of retries is exceeded.</exception>
    /// <returns>A task that represents the asynchronous block operation.</returns>
    [PublicAPI]
    public static async Task WaitUntilAsync(Func<Task<bool>> wait, TimeSpan interval, TimeSpan timeout, int retries = -1, CancellationToken ct = default)
    {
      ushort actualRetries = 0;

      async Task UntilAsync()
      {
        while (!ct.IsCancellationRequested)
        {
          var isSuccessful = await wait.Invoke()
            .ConfigureAwait(false);

          if (isSuccessful)
          {
            break;
          }

          _ = Guard.Argument(retries, nameof(retries))
            .ThrowIf(_ => retries > 0 && ++actualRetries > retries, _ => throw new ArgumentException(MaximumRetryExceededException));

          await Task.Delay(interval, ct)
            .ConfigureAwait(false);
        }
      }

      var waitTask = UntilAsync();

      var timeoutTask = Task.Delay(timeout, ct);

      var isTimeoutTask = timeoutTask == await Task.WhenAny(waitTask, timeoutTask)
        .ConfigureAwait(false);

      if (isTimeoutTask)
      {
        throw new TimeoutException();
      }

      // Rethrows exceptions.
      await waitTask
        .ConfigureAwait(false);
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
