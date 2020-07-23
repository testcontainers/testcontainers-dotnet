namespace DotNet.Testcontainers.Clients
{
  using System;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Services;
  using Microsoft.Extensions.Logging;

  internal sealed class TraceProgress : IProgress<JSONMessage>
  {
    private static readonly ILogger<TraceProgress> Logger = TestcontainersHostService.GetLogger<TraceProgress>();

    public void Report(JSONMessage value)
    {
      Logger.LogTrace(value.ToString());
    }
  }
}
