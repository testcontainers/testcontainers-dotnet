namespace DotNet.Testcontainers.Tests
{
  using System.Runtime.InteropServices;
  using DotNet.Testcontainers.Core.Builder;
  using Xunit;

  public sealed class IgnoreOnLinuxEngine : FactAttribute
  {
    public IgnoreOnLinuxEngine()
    {
      if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || !DockerHostConfiguration.IsWindowsEngineEnabled)
      {
        this.Skip = "Ignore as long as Docker Windows engine is not available.";
      }
    }
  }
}
