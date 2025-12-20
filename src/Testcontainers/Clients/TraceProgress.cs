namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Text.Json;
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
      if (value.Error != null)
      {
        _logger.LogError("ID={ID}: {Error}", value.ID, value.Error.Message);
        return;
      }

      if (!_logger.IsEnabled(LogLevel.Debug))
      {
        return;
      }

      if (!string.IsNullOrWhiteSpace(value.Stream))
      {
        _logger.LogDebug("{Stream}", value.Stream.Trim());
        return;
      }

      if (value.Progress != null && value.Progress.Total > 0)
      {
        var percentage = (double)value.Progress.Current / value.Progress.Total * 100;
        _logger.LogDebug("ID={ID}: {Status} {Percentage,6:F2}% ({Current}/{Total})", value.ID, value.Status, percentage, value.Progress.Current, value.Progress.Total);
        return;
      }

      if (!string.IsNullOrWhiteSpace(value.Status))
      {
        _logger.LogDebug("ID={ID}: {Status}", value.ID, value.Status);
        return;
      }

      if (value.Aux != null)
      {
        _logger.LogDebug("Auxiliary data: {ExtensionData}", JsonSerializer.Serialize(value.Aux.ExtensionData));
      }
    }
  }
}
