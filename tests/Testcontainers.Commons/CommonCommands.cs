namespace DotNet.Testcontainers.Commons;

[PublicAPI]
public static class CommonCommands
{
    public static readonly string[] SleepInfinity = { "/bin/sh", "-c", "trap : TERM INT; sleep infinity & wait" };
}