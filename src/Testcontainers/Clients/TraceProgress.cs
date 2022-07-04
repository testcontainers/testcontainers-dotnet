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
      this.LogProgressIfNotNullOrWhiteSpace(value.Stream);
      this.LogProgressIfNotNullOrWhiteSpace(value.ProgressMessage);
      this.LogProgressIfNotNullOrWhiteSpace(value.Status);
      this.LogErrorIfNotNullOrWhiteSpace(value.ErrorMessage);
    }

    private void LogProgressIfNotNullOrWhiteSpace(string message)
    {
      if (!string.IsNullOrWhiteSpace(message))
      {
        this.logger.Progress(message.Trim());
      }
    }

    private void LogErrorIfNotNullOrWhiteSpace(string message)
    {
      if (!string.IsNullOrWhiteSpace(message))
      {
        this.logger.Error(message.Trim());
      }
    }

  }
}
