namespace DotNet.Testcontainers.Core
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  public static class WaitStrategy
  {
    /// <summary>
    /// Blocks while condition is true or timeout occurs.
    /// </summary>
    /// <param name="frequency">The frequency in milliseconds to check the condition.</param>
    /// <param name="timeout">Timeout in milliseconds.</param>
    /// <exception cref="TimeoutException">Thrown as soon as the timeout expires.</exception>
    /// <returns>A task that represents the asynchronous block operation.</returns>
    public static async Task WaitWhile(Func<Task<bool>> wait, int frequency = 25, int timeout = -1, CancellationToken cancellationToken = default(CancellationToken))
    {
      var waitTask = Task.Run(async () =>
      {
        while (!cancellationToken.IsCancellationRequested && await wait())
        {
          await Task.Delay(frequency);
        }
      });

      if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
      {
        throw new TimeoutException();
      }
    }

    /// <summary>
    /// Blocks until condition is true or timeout occurs.
    /// </summary>
    /// <param name="frequency">The frequency in milliseconds to check the condition.</param>
    /// <param name="timeout">The timeout in milliseconds.</param>
    /// <exception cref="TimeoutException">Thrown as soon as the timeout expires.</exception>
    /// <returns>A task that represents the asynchronous block operation.</returns>
    public static async Task WaitUntil(Func<Task<bool>> wait, int frequency = 25, int timeout = -1, CancellationToken cancellationToken = default(CancellationToken))
    {
      var waitTask = Task.Run(async () =>
      {
        while (!cancellationToken.IsCancellationRequested && !await wait())
        {
          await Task.Delay(frequency);
        }
      });

      if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
      {
        throw new TimeoutException();
      }
    }
  }
}
