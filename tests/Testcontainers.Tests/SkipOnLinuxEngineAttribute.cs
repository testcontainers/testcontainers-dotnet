namespace DotNet.Testcontainers.Tests
{
  using System;
  using System.Diagnostics;
  using Xunit;

  public sealed class SkipOnLinuxEngineAttribute : FactAttribute
  {
    private static readonly bool IsLinuxEngineEnabled = GetIsLinuxEngineEnabled();

    public SkipOnLinuxEngineAttribute()
    {
      if (IsLinuxEngineEnabled)
      {
        this.Skip = "Windows Docker engine is not available.";
      }
    }

    private static bool GetIsLinuxEngineEnabled()
    {
      var dockerProcessStartInfo = new ProcessStartInfo();
      dockerProcessStartInfo.FileName = "docker";
      dockerProcessStartInfo.Arguments = "version --format '{{.Server.Os}}'";
      dockerProcessStartInfo.RedirectStandardOutput = true;
      dockerProcessStartInfo.UseShellExecute = false;

      var dockerProcess = new Process();
      dockerProcess.StartInfo = dockerProcessStartInfo;

      try
      {
        if (dockerProcess.Start())
        {
          var stdout = dockerProcess.StandardOutput.ReadToEnd();
          return stdout.Contains("linux", StringComparison.OrdinalIgnoreCase);
        }
        else
        {
          throw new InvalidOperationException("Docker not found.");
        }
      }
      finally
      {
        dockerProcess.Dispose();
      }
    }
  }
}
