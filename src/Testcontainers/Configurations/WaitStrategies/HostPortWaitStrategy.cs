namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Net.Sockets;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// Waits until a TCP connection is possible from Host to container
  /// </summary>
  internal sealed class HostPortWaitStrategy(int port) : IWaitUntil
  {
    private readonly UntilTcpConnected _tcpCheck = new(port);

    public async Task<bool> UntilAsync(IContainer container)
    {
      // Then check if connection can be established from host
      return await _tcpCheck.UntilAsync(container).ConfigureAwait(false);
    }
  }
}
