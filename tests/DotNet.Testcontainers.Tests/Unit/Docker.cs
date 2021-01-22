namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Diagnostics;

  public static class Docker
  {
    public static bool Exists(DockerResource dockerResource, string name)
    {
      var dockerProcessStartInfo = new ProcessStartInfo();
      dockerProcessStartInfo.FileName = "docker";
      dockerProcessStartInfo.Arguments = $"inspect --type={dockerResource.Type} {name}";
      dockerProcessStartInfo.RedirectStandardOutput = true;
      dockerProcessStartInfo.RedirectStandardError = true;
      dockerProcessStartInfo.UseShellExecute = false;

      var dockerProcess = Process.Start(dockerProcessStartInfo);

      if (dockerProcess == null)
      {
        return false;
      }
      else
      {
        _ = dockerProcess.StandardOutput.ReadToEnd();
        _ = dockerProcess.StandardError.ReadToEnd();

        dockerProcess.WaitForExit();
        return 0.Equals(dockerProcess.ExitCode);
      }
    }
  }
}
