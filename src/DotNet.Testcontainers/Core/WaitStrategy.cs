namespace DotNet.Testcontainers.Core
{
  using System;
  using System.Threading.Tasks;

  internal class WaitStrategy
  {
    private WaitStrategy()
    {
    }

    /// <summary>
    /// Blocks while condition is true or timeout occurs.
    /// </summary>
    /// <param name="condition">The condition that will perpetuate the block.</param>
    /// <param name="frequency">The frequency in milliseconds to check the condition.</param>
    /// <param name="timeout">Timeout in milliseconds.</param>
    /// <exception cref="TimeoutException">Thrown as soon as the timeout expires.</exception>
    /// <returns>A task that represents the asynchronous block operation.</returns>
    public static async Task WaitWhile(Func<bool> condition, int frequency = 25, int timeout = -1)
    {
      var waitTask = Task.Run(async () =>
      {
        while (condition())
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
    /// <param name="condition">The break condition.</param>
    /// <param name="frequency">The frequency in milliseconds to check the condition.</param>
    /// <param name="timeout">The timeout in milliseconds.</param>
    /// <exception cref="TimeoutException">Thrown as soon as the timeout expires.</exception>
    /// <returns>A task that represents the asynchronous block operation.</returns>
    public static async Task WaitUntil(Func<bool> condition, int frequency = 25, int timeout = -1)
    {
      var waitTask = Task.Run(async () =>
      {
        while (!condition())
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
