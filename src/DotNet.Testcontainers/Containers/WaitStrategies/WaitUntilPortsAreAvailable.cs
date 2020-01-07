namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System.Linq;

  internal class WaitUntilPortsAreAvailable : WaitUntilShellCommandsAreCompleted
  {
    public WaitUntilPortsAreAvailable(params int[] ports) :
      base(ports.Select(port => $"true && (cat /proc/net/tcp{{,6}} | awk '{{print $2}}' | grep -i :{port} || nc -vz -w 1 localhost {port} || /bin/bash -c '</dev/tcp/localhost/{port}')").ToArray())
    {
    }
  }
}
