namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
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

    public virtual async Task<bool> Until(Uri endpoint, string id, ILogger logger)
    {
      var client = new TestcontainersClient(endpoint, logger);

      var result = await client.ExecAsync(id, this.command)
        .ConfigureAwait(false);

      return 0L.Equals(result.ExitCode);
    }
  }
}
