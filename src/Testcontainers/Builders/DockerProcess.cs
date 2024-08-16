using System;
using System.Diagnostics;

namespace DotNet.Testcontainers.Builders
{
  internal static class DockerProcess
  {
    public static Uri GetCurrentEndpoint()
    {
      var dockerStartInfo = new ProcessStartInfo();
      dockerStartInfo.FileName = "docker";
      dockerStartInfo.Arguments = "context inspect --format {{.Endpoints.docker.Host}}";
      dockerStartInfo.RedirectStandardOutput = true;
      dockerStartInfo.UseShellExecute = false;

      var dockerProcess = new Process();
      dockerProcess.StartInfo = dockerStartInfo;

      try
      {
        if (dockerProcess.Start())
        {
          dockerProcess.WaitForExit(2000);
          if (dockerProcess.ExitCode == 0)
          {
            var endpoint = dockerProcess.StandardOutput.ReadToEnd().Trim();
            return new Uri(endpoint);
          }
        }
        return null;
      }
      catch
      {
        return null;
      }
      finally
      {
        dockerProcess.Dispose();
      }
    }
  }
}
