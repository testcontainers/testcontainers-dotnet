using System.Net.Sockets;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;

namespace DotNet.Testcontainers.Configurations
{
    /// <summary>
    /// Wait for a TCP connection to be established.
    /// </summary>s
    internal sealed class UntilTcpConnected(int port) : IWaitUntil
    {
        private readonly int _containerPort = port;

        public async Task<bool> UntilAsync(IContainer container)
        {
            ushort hostPort;
            try
            {
                hostPort = container.GetMappedPublicPort(_containerPort);
            }
            catch
            {
                return false;
            }

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