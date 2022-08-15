namespace DotNet.Testcontainers.Configurations
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  internal class UntilUnixCommandIsCompleted : IWaitUntil
  {
    private readonly string[] command;

    public UntilUnixCommandIsCompleted(string command)
      : this("/bin/sh", "-c", command)
    {
    }

    public UntilUnixCommandIsCompleted(params string[] command)
    {
      this.command = command;
    }

    public virtual async Task<bool> Until(ITestcontainersContainer testcontainers, ILogger logger)
    {
      var execResult = await testcontainers.ExecAsync(this.command)
        .ConfigureAwait(false);

      return 0L.Equals(execResult.ExitCode);
    }
  }
}
