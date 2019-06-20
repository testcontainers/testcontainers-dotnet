namespace DotNet.Testcontainers.Diagnostics
{
  using System;
  using System.Diagnostics;
  using Docker.DotNet.Models;

  internal sealed class DebugProgress : IProgress<JSONMessage>
  {
    private static readonly Lazy<DebugProgress> progress = new Lazy<DebugProgress>(() => new DebugProgress());

    public static DebugProgress Instance => progress.Value;

    public void Report(JSONMessage value)
    {
      Debug.WriteLine(value.ToString());
    }
  }
}
