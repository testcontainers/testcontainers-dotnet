namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Diagnostics;

  public static class Docker
  {
    public static bool Exists(DockerResource dockerResource, string name)
    {
      DataReceivedEventHandler ignoreReceivedData = (_, _) => { };

      var dockerProcessStartInfo = new ProcessStartInfo();
      dockerProcessStartInfo.FileName = "docker";
      dockerProcessStartInfo.Arguments = $"inspect --type={dockerResource.Type} {name}";
      dockerProcessStartInfo.RedirectStandardError = true;
      dockerProcessStartInfo.RedirectStandardOutput = true;
      dockerProcessStartInfo.UseShellExecute = false;

      var dockerProcess = new Process();
      dockerProcess.StartInfo = dockerProcessStartInfo;
      dockerProcess.ErrorDataReceived += ignoreReceivedData;
      dockerProcess.OutputDataReceived += ignoreReceivedData;

      try
      {
        if (dockerProcess.Start())
        {
          dockerProcess.BeginErrorReadLine();
          dockerProcess.BeginOutputReadLine();
          dockerProcess.WaitForExit();
          return 0.Equals(dockerProcess.ExitCode);
        }
        else
        {
          throw new InvalidOperationException("Docker not found.");
        }
      }
      finally
      {
        dockerProcess.ErrorDataReceived -= ignoreReceivedData;
        dockerProcess.OutputDataReceived -= ignoreReceivedData;
        dockerProcess.Dispose();
      }
    }
  }
}
