namespace DotNet.Testcontainers.Tests
{
  using System.Runtime.InteropServices;
  using Xunit;

  public sealed class SkipOnLinuxOSAttribute : FactAttribute
  {
    private static readonly bool IsLinuxOS = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    public SkipOnLinuxOSAttribute()
    {
      if (IsLinuxOS)
      {
        this.Skip = "Docker host is running linux.";
      }
    }
  }
}
