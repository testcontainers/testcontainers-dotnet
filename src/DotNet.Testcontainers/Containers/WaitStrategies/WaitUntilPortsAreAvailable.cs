namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System.Linq;

  internal class WaitUntilPortsAreAvailable : WaitUntilBashCommandsAreCompleted
  {
    public WaitUntilPortsAreAvailable(params int[] ports) :
      base(ports.Select(port => $"timeout 15 bash -c \"echo > /dev/tcp/localhost/{port}\"").ToArray())
    {
    }
  }
}
