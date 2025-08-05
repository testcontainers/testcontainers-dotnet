using System.Net.Sockets;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;

namespace DotNet.Testcontainers.Configurations
{
  /// <summary>
  /// Wait for a TCP connection to be established.
  /// </summary>
  /// <remarks>
  /// Some docker configurations (DockerForMac & Rancher on Mac & Windows) allow establishing a connection to any mapped port.
  /// https://forums.docker.com/t/port-forwarding-of-exposed-ports-behaves-unexpectedly/15807
  /// In these cases, the mapped port is always available even if the container is not listening on it.
  /// </remarks>
  /// <param name="port">The container port to check.</param>
  internal sealed class UntilHostTcpPortIsAvailable(int port) : IWaitUntil
  {
    // We don't have access to all mapped ports, so we need the user to provide the container port.
    private readonly int _containerPort = port;

    public async Task<bool> UntilAsync(IContainer container)
    {
      var hostPort = container.GetMappedPublicPort(_containerPort);

      using var tcpClient = new TcpClient();
      try
      {
        await tcpClient.ConnectAsync(container.Hostname, hostPort)
            .ConfigureAwait(false);
        return tcpClient.Connected;
      }
      catch
      {
        return false;
      }
    }
  }
}
