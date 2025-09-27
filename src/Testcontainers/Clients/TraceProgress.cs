namespace DotNet.Testcontainers.Clients
{
  using System;
  using Docker.DotNet.Models;
  using Microsoft.Extensions.Logging;

  internal sealed class TraceProgress : IProgress<JSONMessage>
  {
    private readonly ILogger _logger;

    public TraceProgress(ILogger logger)
    {
      _logger = logger;
    }

    public void Report(JSONMessage value)
    {
#pragma warning disable CA1848, CA2254

      if (!string.IsNullOrWhiteSpace(value.Status))
      {
        _logger.LogDebug(value.Status.TrimEnd());
      }

      if (!string.IsNullOrWhiteSpace(value.Stream))
      {
        _logger.LogDebug(value.Stream.TrimEnd());
      }

      if (!string.IsNullOrWhiteSpace(value.ProgressMessage))
      {
        _logger.LogDebug(value.ProgressMessage.TrimEnd());
      }

      if (!string.IsNullOrWhiteSpace(value.ErrorMessage))
      {
        _logger.LogError(value.ErrorMessage.TrimEnd());
      }

#pragma warning restore CA1848, CA2254
    }
  }
}
