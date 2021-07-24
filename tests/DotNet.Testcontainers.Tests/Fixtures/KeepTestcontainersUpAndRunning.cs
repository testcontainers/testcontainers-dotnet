namespace DotNet.Testcontainers.Tests.Fixtures
{
  public static class KeepTestcontainersUpAndRunning
  {
    public static string[] Command { get; }
      = { "/bin/sh", "-c", "tail -f /dev/null" };
  }
}
