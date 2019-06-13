namespace DotNet.Testcontainers.Tests
{
  using System.Runtime.InteropServices;
  using Xunit;

  public sealed class IgnoreOnLinuxEngine : FactAttribute
  {
    public IgnoreOnLinuxEngine()
    {
      if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || !this.IsWindowsEngineEnabled)
      {
        this.Skip = "Ignore as long as Docker Windows engine is not available.";
      }
    }

    public bool IsWindowsEngineEnabled
    {
      get
      {
        return false; // TODO: Check if Docker Windows engine is available.
      }
    }
  }
}
