namespace DotNet.Testcontainers.Services
{
  using System.Net;
  using System.Net.Sockets;

  public static class TestcontainersNetworkService
  {
    private static readonly IPEndPoint DefaultLoopbackEndpoint = new IPEndPoint(IPAddress.Loopback, 0);

    public static int GetAvailablePort()
    {
      using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
      {
        socket.Bind(DefaultLoopbackEndpoint);
        return ((IPEndPoint)socket.LocalEndPoint).Port;
      }
    }
  }
}
