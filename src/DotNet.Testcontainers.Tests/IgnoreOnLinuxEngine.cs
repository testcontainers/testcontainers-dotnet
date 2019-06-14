namespace DotNet.Testcontainers.Tests
{
  using System;
  using System.Runtime.InteropServices;
  using Xunit;

  public sealed class IgnoreOnLinuxEngine : FactAttribute
  {
    private static readonly bool IsWindowsEngineEnabled = "Windows_NT".Equals(Environment.GetEnvironmentVariable("AGENT_OS")); // TODO: Replace this with and Docker API call.

    public IgnoreOnLinuxEngine()
    {
      if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || !IsWindowsEngineEnabled)
      {
        this.Skip = "Ignore as long as Docker Windows engine is not available.";
      }
    }
  }
}
