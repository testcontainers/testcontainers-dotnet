namespace DotNet.Testcontainers.Configurations
{
  internal class UntilWindowsPortIsAvailable : UntilWindowsCommandIsCompleted
  {
    public UntilWindowsPortIsAvailable(int port)
      : base($"Exit !(Test-NetConnection -ComputerName 'localhost' -Port {port}).TcpTestSucceeded")
    {
    }
  }
}
