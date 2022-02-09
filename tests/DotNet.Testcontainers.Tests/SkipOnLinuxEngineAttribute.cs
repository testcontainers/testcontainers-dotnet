namespace DotNet.Testcontainers.Tests
{
  using System;
  using System.Diagnostics;
  using Xunit;

  public sealed class SkipOnLinuxEngineAttribute : FactAttribute
  {
    public SkipOnLinuxEngineAttribute()
    {
      if (GetIsLinuxEngineEnabled())
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

      var dockerProcess = Process.Start(dockerProcessStartInfo);

      if (dockerProcess == null)
      {
        throw new InvalidOperationException("Docker not found.");
      }

      var stdout = dockerProcess.StandardOutput.ReadToEnd();
      return stdout.Contains("linux", StringComparison.OrdinalIgnoreCase);
    }
  }
}
