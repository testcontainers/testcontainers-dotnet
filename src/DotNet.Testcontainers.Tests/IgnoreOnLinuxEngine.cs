namespace DotNet.Testcontainers.Tests
{
  using System.Runtime.InteropServices;
  using DotNet.Testcontainers.Clients;
  using Xunit;

  public sealed class IgnoreOnLinuxEngine : FactAttribute
  {
    public IgnoreOnLinuxEngine()
    {
      if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || !new TestcontainersClient().GetIsWindowsEngineEnabled().GetAwaiter().GetResult())
      {
        this.Skip = "Ignore as long as Docker Windows engine is not available.";
      }
    }
  }
}
