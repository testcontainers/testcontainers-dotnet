namespace DotNet.Testcontainers.Core
{
  using System;
  using System.Threading.Tasks;

  public abstract class WaitStrategy
  {
    protected WaitStrategy()
    {
    }

    /// <summary>
    /// Blocks while condition is true or timeout occurs.
    /// </summary>
    /// <param name="frequency">The frequency in milliseconds to check the condition.</param>
    /// <param name="timeout">Timeout in milliseconds.</param>
    /// <exception cref="TimeoutException">Thrown as soon as the timeout expires.</exception>
    /// <returns>A task that represents the asynchronous block operation.</returns>
    public async Task WaitWhile(int frequency = 25, int timeout = -1)
    {
      var waitTask = Task.Run(async () =>
      {
        while (await this.While())
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
    public async Task WaitUntil(int frequency = 25, int timeout = -1)
    {
      var waitTask = Task.Run((Func<Task>)(async () =>
      {
        while (!await this.Until())
        {
          await Task.Delay(frequency);
        }
      }));

      if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
      {
        throw new TimeoutException();
      }
    }

    protected abstract Task<bool> While();

    protected abstract Task<bool> Until();
  }
}
