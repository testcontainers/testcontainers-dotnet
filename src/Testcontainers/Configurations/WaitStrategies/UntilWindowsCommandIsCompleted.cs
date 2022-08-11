// ReSharper disable once CheckNamespace to avoid introducing breaking changes
namespace DotNet.Testcontainers.Configurations
{
  internal class UntilWindowsCommandIsCompleted : UntilUnixCommandIsCompleted
  {
    public UntilWindowsCommandIsCompleted(string command)
      : this("PowerShell", "-Command", command)
    {
    }

    public UntilWindowsCommandIsCompleted(params string[] command)
      : base(command)
    {
    }
  }
}
