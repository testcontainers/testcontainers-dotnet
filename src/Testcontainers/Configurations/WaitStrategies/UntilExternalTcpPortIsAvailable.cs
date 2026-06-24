namespace DotNet.Testcontainers.Configurations
{
  using System.Net.Sockets;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;

  internal class UntilExternalTcpPortIsAvailable : IWaitUntil
  {
    private readonly int _containerPort;

    public UntilExternalTcpPortIsAvailable(int containerPort)
    {
      _containerPort = containerPort;
    }

    public async Task<bool> UntilAsync(IContainer container)
    {
      var hostPort = container.GetMappedPublicPort(_containerPort);

      var tcpClient = new TcpClient();

      try
      {
        await tcpClient.ConnectAsync(container.Hostname, hostPort)
          .ConfigureAwait(false);

        return true;
      }
      catch
      {
        return false;
      }
      finally
      {
        tcpClient.Dispose();
      }
    }
  }
}
