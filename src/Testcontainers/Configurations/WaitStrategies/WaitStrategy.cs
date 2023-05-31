namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using JetBrains.Annotations;

  public static class WaitStrategy
  {
    /// <summary>
    /// Waits asynchronously until the specified condition returns false or until a timeout occurs.
    /// </summary>
    /// <param name="wait">A function that represents the asynchronous condition to wait for.</param>
    /// <param name="interval">The time interval between consecutive evaluations of the condition.</param>
    /// <param name="timeout">The maximum duration to wait for the condition to become false.</param>
    /// <param name="ct">The optional cancellation token to cancel the waiting operation.</param>
    /// <exception cref="TimeoutException">Thrown as soon as the timeout expires.</exception>
    /// <returns>A task that represents the asynchronous block operation.</returns>
    [PublicAPI]
    public static async Task WaitWhileAsync(Func<Task<bool>> wait, TimeSpan interval, TimeSpan timeout, CancellationToken ct = default)
    {
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
    /// <param name="wait">A function that represents the asynchronous condition to wait for.</param>
    /// <param name="interval">The time interval between consecutive evaluations of the condition.</param>
    /// <param name="timeout">The maximum duration to wait for the condition to become true.</param>
    /// <param name="ct">The optional cancellation token to cancel the waiting operation.</param>
    /// <exception cref="TimeoutException">Thrown as soon as the timeout expires.</exception>
    /// <returns>A task that represents the asynchronous block operation.</returns>
    [PublicAPI]
    public static async Task WaitUntilAsync(Func<Task<bool>> wait, TimeSpan interval, TimeSpan timeout, CancellationToken ct = default)
    {
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
  }
}
