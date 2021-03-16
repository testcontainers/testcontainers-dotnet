namespace DotNet.Testcontainers.Containers.WaitStrategies.Unix
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Clients;

  internal class UntilCommandIsCompleted : IWaitUntil
  {
    private readonly string[] command;

    public UntilCommandIsCompleted(string command) : this("/bin/sh", "-c", command)
    {
    }

    public UntilCommandIsCompleted(params string[] command)
    {
      this.command = command;
    }

    public virtual async Task<bool> Until(IDockerClientConfiguration clientAuthConfig, string id)
    {
      using (var client = new TestcontainersClient(clientAuthConfig))
      {
        var exitCode = await client.ExecAsync(id, this.command)
          .ConfigureAwait(false);
        return 0L.Equals(exitCode);
      }
    }
  }
}
