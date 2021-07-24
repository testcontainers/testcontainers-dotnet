namespace DotNet.Testcontainers.Configurations
{
  internal class UntilUnixPortIsAvailable : UntilUnixCommandIsCompleted
  {
    public UntilUnixPortIsAvailable(int port)
      : base($"true && (cat /proc/net/tcp{{,6}} | awk '{{print $2}}' | grep -i :{port} || nc -vz -w 1 localhost {port} || /bin/bash -c '</dev/tcp/localhost/{port}')")
    {
    }
  }
}
