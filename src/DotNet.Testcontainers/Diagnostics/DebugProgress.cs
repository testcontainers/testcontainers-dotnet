namespace DotNet.Testcontainers.Diagnostics
{
  using System;
  using System.Diagnostics;
  using Docker.DotNet.Models;

  internal sealed class DebugProgress : IProgress<JSONMessage>
  {
    private static readonly Lazy<DebugProgress> Progress = new Lazy<DebugProgress>(() => new DebugProgress());

    public static DebugProgress Instance => Progress.Value;

    public void Report(JSONMessage value)
    {
      Debug.WriteLine(value.ToString());
    }
  }
}
