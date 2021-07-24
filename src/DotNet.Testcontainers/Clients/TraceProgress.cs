namespace DotNet.Testcontainers.Clients
{
  using System;
  using Docker.DotNet.Models;
  using Microsoft.Extensions.Logging;

  internal sealed class TraceProgress : IProgress<JSONMessage>
  {
    private readonly ILogger logger;

    public TraceProgress(ILogger logger)
    {
      this.logger = logger;
    }

    public void Report(JSONMessage value)
    {
      this.logger.LogTrace(value.ToString());
    }
  }
}
