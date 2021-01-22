namespace DotNet.Testcontainers.Tests.Fixtures
{
  public static class KeepTestcontainersUpAndRunning
  {
    public static string[] Command { get; }
      = { "/bin/sh", "-c", "trap \"exit\" TERM; while true; do sleep 1; done;" };
  }
}
