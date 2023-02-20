namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using JetBrains.Annotations;

  public static class WaitStrategy
  {
    /// <summary>
    /// Blocks while condition is true or timeout occurs.
    /// </summary>
    /// <param name="wait">Function to block execution.</param>
    /// <param name="frequency">The frequency in milliseconds to check the condition.</param>
    /// <param name="timeout">Timeout in milliseconds.</param>
    /// <param name="ct">Propagates notification that operations should be canceled.</param>
    /// <exception cref="TimeoutException">Thrown as soon as the timeout expires.</exception>
    /// <returns>A task that represents the asynchronous block operation.</returns>
    [PublicAPI]
    public static async Task WaitWhileAsync(Func<Task<bool>> wait, TimeSpan frequency, TimeSpan timeout, CancellationToken ct = default)
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

          await Task.Delay(frequency, ct)
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
    /// Blocks until condition is true or timeout occurs.
    /// </summary>
    /// <param name="wait">Function to block execution.</param>
    /// <param name="frequency">The frequency in milliseconds to check the condition.</param>
    /// <param name="timeout">The timeout in milliseconds.</param>
    /// <param name="ct">Propagates notification that operations should be canceled.</param>
    /// <exception cref="TimeoutException">Thrown as soon as the timeout expires.</exception>
    /// <returns>A task that represents the asynchronous block operation.</returns>
    [PublicAPI]
    public static async Task WaitUntilAsync(Func<Task<bool>> wait, TimeSpan frequency, TimeSpan timeout, CancellationToken ct = default)
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

          await Task.Delay(frequency, ct)
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
