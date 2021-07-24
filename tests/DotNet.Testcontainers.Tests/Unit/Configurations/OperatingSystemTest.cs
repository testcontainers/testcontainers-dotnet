namespace DotNet.Testcontainers.Tests.Unit
{
  using DotNet.Testcontainers.Configurations;
  using Xunit;

  public static class OperatingSystemTest
  {
    public sealed class UnixTest
    {
      [Fact]
      public void NormalizePath()
      {
        IOperatingSystem os = default(Unix);
        Assert.Equal("/foo/bar", os.NormalizePath("\\foo\\bar"));
      }
    }

    public sealed class WindowsTest
    {
      [Fact]
      public void NormalizePath()
      {
        IOperatingSystem os = default(Windows);
        Assert.Equal("\\foo\\bar", os.NormalizePath("/foo/bar"));
      }
    }
  }
}
