namespace DotNet.Testcontainers.Containers.WaitStrategies.Windows
{
  internal class UntilPortIsAvailable : UntilCommandIsCompleted
  {
    public UntilPortIsAvailable(int port) : base(
      $"exit !(Test-NetConnection -ComputerName 'localhost' -Port {port}).TcpTestSucceeded")
    {
    }
  }
}
