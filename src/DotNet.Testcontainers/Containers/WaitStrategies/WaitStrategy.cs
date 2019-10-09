namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  internal static class WaitStrategy
  {
    /// <summary>
    /// Blocks while condition is true or timeout occurs.
    /// </summary>
    /// <param name="wait">Function to block execution.</param>
    /// <param name="frequency">The frequency in milliseconds to check the condition.</param>
    /// <param name="timeout">Timeout in milliseconds.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <exception cref="TimeoutException">Thrown as soon as the timeout expires.</exception>
    /// <returns>A task that represents the asynchronous block operation.</returns>
    public static async Task WaitWhile(Func<Task<bool>> wait, int frequency = 25, int timeout = -1, CancellationToken cancellationToken = default)
    {
      var waitTask = Task.Run(async () =>
      {
        while (!cancellationToken.IsCancellationRequested && await wait())
        {
          await Task.Delay(frequency, cancellationToken);
        }
      }, cancellationToken);

      await RethrowPotentialException(await Task.WhenAny(waitTask, Task.Delay(timeout, cancellationToken)), waitTask);
    }

    /// <summary>
    /// Blocks until condition is true or timeout occurs.
    /// </summary>
    /// <param name="wait">Function to block execution.</param>
    /// <param name="frequency">The frequency in milliseconds to check the condition.</param>
    /// <param name="timeout">The timeout in milliseconds.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <exception cref="TimeoutException">Thrown as soon as the timeout expires.</exception>
    /// <returns>A task that represents the asynchronous block operation.</returns>
    public static async Task WaitUntil(Func<Task<bool>> wait, int frequency = 25, int timeout = -1, CancellationToken cancellationToken = default)
    {
      var waitTask = Task.Run(async () =>
      {
        while (!cancellationToken.IsCancellationRequested && !await wait())
        {
          await Task.Delay(frequency, cancellationToken);
        }
      }, cancellationToken);

      await RethrowPotentialException(await Task.WhenAny(waitTask, Task.Delay(timeout, cancellationToken)), waitTask);
    }

    private static Task RethrowPotentialException(Task completedTask, Task waitTask)
    {
      return completedTask == waitTask ? completedTask : throw new TimeoutException();
    }
  }
}
