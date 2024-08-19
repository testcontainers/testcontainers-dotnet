namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Diagnostics;
  using DotNet.Testcontainers.Builders;
  using Xunit;

  public class DockerConfigTest
  {
    [Fact]
    public void GetCurrentEndpoint()
    {
      var endpoint = DockerConfig.Default.GetCurrentEndpoint();
      Assert.NotNull(endpoint);

      var expectedEndpoint = DockerProcess.GetCurrentEndpoint();
      Assert.Equal(expectedEndpoint, endpoint);
    }
  }

  internal static class DockerProcess
  {
    public static Uri GetCurrentEndpoint()
    {
      using var docker = new Process();
      docker.StartInfo = new ProcessStartInfo
      {
        FileName = "docker",
        Arguments = "context inspect --format {{.Endpoints.docker.Host}}",
        RedirectStandardOutput = true,
        UseShellExecute = false,
      };
      docker.Start();
      docker.WaitForExit(2000);
      var endpoint = docker.StandardOutput.ReadToEnd().Trim();
      return new Uri(endpoint);
    }
  }
}
