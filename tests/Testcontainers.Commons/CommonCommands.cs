namespace DotNet.Testcontainers.Commons
{
  using JetBrains.Annotations;

  [PublicAPI]
  public static class CommonCommands
  {
    public static readonly string[] SleepInfinity = { "/bin/sh", "-c", "trap : TERM INT; sleep infinity & wait" };
  }
}
