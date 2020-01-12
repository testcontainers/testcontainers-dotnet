namespace DotNet.Testcontainers.Tests
{
  using System.Runtime.InteropServices;
  using DotNet.Testcontainers.Clients;
  using Xunit;

  public sealed class IgnoreOnLinuxEngine : FactAttribute
  {
    private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    private static readonly bool IsWindowsEngineEnabled = new TestcontainersClient().GetIsWindowsEngineEnabled().GetAwaiter().GetResult();

    public IgnoreOnLinuxEngine()
    {
      if (!IsWindows || !IsWindowsEngineEnabled)
      {
        this.Skip = "Ignore as long as Docker Windows engine is not available.";
      }
    }
  }
}
