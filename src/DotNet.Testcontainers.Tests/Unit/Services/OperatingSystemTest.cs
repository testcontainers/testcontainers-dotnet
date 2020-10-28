namespace DotNet.Testcontainers.Tests.Unit.Services
{
  using DotNet.Testcontainers.Services;
  using Xunit;

  public static class OperatingSystemTest
  {
    public class UnixTest
    {
      [Fact]
      public void NormalizePath()
      {
        IOperatingSystem os = new Unix();
        Assert.Equal("/foo/bar", os.NormalizePath("\\foo\\bar"));
      }
    }

    public class WindowsTest
    {
      [Fact]
      public void NormalizePath()
      {
        IOperatingSystem os = new Windows();
        Assert.Equal("\\foo\\bar", os.NormalizePath("/foo/bar"));
      }
    }
  }
}
