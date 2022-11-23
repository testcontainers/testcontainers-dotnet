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
#pragma warning disable CA1848, CA2254

      if (!string.IsNullOrWhiteSpace(value.ProgressMessage))
      {
        this.logger.LogTrace(value.ProgressMessage);
      }

      if (!string.IsNullOrWhiteSpace(value.ErrorMessage))
      {
        this.logger.LogError(value.ErrorMessage);
      }

#pragma warning restore CA1848, CA2254
    }
  }
}
