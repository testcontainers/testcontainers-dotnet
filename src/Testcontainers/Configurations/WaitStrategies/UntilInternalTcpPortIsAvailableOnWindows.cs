namespace DotNet.Testcontainers.Configurations
{
  internal class UntilInternalTcpPortIsAvailableOnWindows : UntilWindowsCommandIsCompleted
  {
    public UntilInternalTcpPortIsAvailableOnWindows(int containerPort)
      : base(
        $"Exit(-Not((Test-NetConnection -ComputerName 'localhost' -Port {containerPort}).TcpTestSucceeded))"
      ) { }
  }
}
