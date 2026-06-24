namespace DotNet.Testcontainers.Configurations
{
  internal class UntilInternalTcpPortIsAvailableOnUnix : UntilUnixCommandIsCompleted
  {
    public UntilInternalTcpPortIsAvailableOnUnix(int containerPort)
      : base(string.Format("true && (grep -i ':0*{0:X}' /proc/net/tcp* || nc -vz -w 1 localhost {0:D} || /bin/bash -c '</dev/tcp/localhost/{0:D}')", containerPort))
    {
    }
  }
}
