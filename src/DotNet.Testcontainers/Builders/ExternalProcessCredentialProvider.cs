namespace DotNet.Testcontainers.Builders
{
  using System.Diagnostics;

  internal static class ExternalProcessCredentialProvider
  {
    public static string GetCredentialProviderOutput(string credentialProviderName, string hostname)
    {
      var dockerCredentialStartInfo = new ProcessStartInfo();
      dockerCredentialStartInfo.FileName = "docker-credential-" + credentialProviderName;
      dockerCredentialStartInfo.Arguments = "get";
      dockerCredentialStartInfo.RedirectStandardInput = true;
      dockerCredentialStartInfo.RedirectStandardOutput = true;
      dockerCredentialStartInfo.UseShellExecute = false;

      using (var dockerCredentialProcess = Process.Start(dockerCredentialStartInfo))
      {
        if (dockerCredentialProcess == null)
        {
          return null;
        }

        dockerCredentialProcess.StandardInput.WriteLine(hostname);
        dockerCredentialProcess.StandardInput.Close();

        return dockerCredentialProcess.StandardOutput.ReadToEnd().Trim();
      }
    }
  }
}
