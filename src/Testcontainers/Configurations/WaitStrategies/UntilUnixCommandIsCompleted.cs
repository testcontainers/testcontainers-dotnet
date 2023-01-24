namespace DotNet.Testcontainers.Configurations
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;

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

    public virtual async Task<bool> UntilAsync(IContainer container)
    {
      var execResult = await container.ExecAsync(this.command)
        .ConfigureAwait(false);

      return 0L.Equals(execResult.ExitCode);
    }
  }
}
