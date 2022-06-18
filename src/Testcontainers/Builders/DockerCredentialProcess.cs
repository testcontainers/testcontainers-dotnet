namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Diagnostics;

  internal static class DockerCredentialProcess
  {
    public static string Get(string credentialProviderName, string hostname)
    {
      var dockerCredentialStartInfo = new ProcessStartInfo();
      dockerCredentialStartInfo.FileName = "docker-credential-" + credentialProviderName;
      dockerCredentialStartInfo.Arguments = "get";
      dockerCredentialStartInfo.RedirectStandardInput = true;
      dockerCredentialStartInfo.RedirectStandardOutput = true;
      dockerCredentialStartInfo.UseShellExecute = false;

      var dockerCredentialProcess = new Process();
      dockerCredentialProcess.StartInfo = dockerCredentialStartInfo;

      try
      {
        if (dockerCredentialProcess.Start())
        {
          dockerCredentialProcess.StandardInput.WriteLine(hostname);
          dockerCredentialProcess.StandardInput.Close();
          return dockerCredentialProcess.StandardOutput.ReadToEnd().Trim();
        }
        else
        {
          throw new InvalidOperationException("Docker not found.");
        }
      }
      catch (Exception)
      {
        return null;
      }
      finally
      {
        dockerCredentialProcess.Dispose();
      }
    }
  }
}
