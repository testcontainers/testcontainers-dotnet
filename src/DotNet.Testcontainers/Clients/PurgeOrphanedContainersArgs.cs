namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  internal readonly struct PurgeOrphanedContainersArgs
  {
    private readonly StringBuilder argsBuilder;

    public PurgeOrphanedContainersArgs(Uri endpoint, IEnumerable<string> registeredContainers) : this(
      DockerApiEndpoint.Local.Equals(endpoint) ? string.Empty : endpoint.ToString(), string.Join(" ", registeredContainers))
    {
    }

    private PurgeOrphanedContainersArgs(string endpoint, string registeredContainers)
    {
      this.argsBuilder = new StringBuilder();

      if (endpoint.Any())
      {
        this.argsBuilder
          .Append("-H ")
          .Append(endpoint);
      }

      if (registeredContainers.Any())
      {
        this.argsBuilder
          .Append("rm --force ")
          .Append(registeredContainers);
      }
    }

    public override string ToString()
    {
      return this.argsBuilder.ToString();
    }
  }
}
