namespace DotNet.Testcontainers.Clients
{
  using System;
  using Docker.DotNet.Models;

  internal sealed class DebugProgress : IProgress<JSONMessage>
  {
    public static readonly DebugProgress Provider = new DebugProgress();

    private DebugProgress()
    {
    }

    public void Report(JSONMessage value)
    {
      System.Diagnostics.Debug.WriteLine(value.ToString());
    }
  }
}
