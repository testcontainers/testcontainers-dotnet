namespace DotNet.Testcontainers.Configurations
{
  internal class UntilInternalTcpPortIsAvailableOnWindows : UntilUnixCommandIsCompleted
  {
    public UntilInternalTcpPortIsAvailableOnWindows(int containerPort)
      : base($"Exit(-Not((Test-NetConnection -ComputerName 'localhost' -Port {containerPort}).TcpTestSucceeded))")
    {
    }
  }
}
